namespace Blocktrust.PeerDID.PeerDIDCreateResolve;

using Exceptions;
using Core;
using DIDDoc;
using FluentResults;
using Types;

public static class PeerDidResolver
{
    public static Result<string> ResolvePeerDid(PeerDid peerDid, VerificationMaterialFormatPeerDid format = VerificationMaterialFormatPeerDid.Multibase)
    {
        if (!PeerDidCreator.IsPeerDid(peerDid.Value))
        {
            return Result.Fail($"Does not match peer DID regexp: {peerDid}");
        }

        DidDocPeerDid didDoc;
        try 
        {
            didDoc = peerDid.Value[9] switch 
            {
                '0' => BuildDidDocNumalgo0(peerDid, format),
                '2' => BuildDidDocNumalgo2(peerDid, format),
                _ => throw new ArgumentException($"Invalid numalgo of Peer DID: {peerDid}")
            };
        }
        catch (Exception e)
        {
            return Result.Fail($"Error resolving Peer DID: {e.Message}");
        }

        return Result.Ok(didDoc.ToJson());
    }

    private static DidDocPeerDid BuildDidDocNumalgo0(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        var inceptionKey = peerDid.Value.Substring(10);
        var decodedEncumbasis = DecodeMultibaseEncnumbasisAuth(inceptionKey, format);
        
        return new DidDocPeerDid(
            did: peerDid.Value,
            authentications: new List<VerificationMethodPeerDid>
            {
                PeerDidHelper.GetVerificationMethod(peerDid.Value, decodedEncumbasis)
            });
    }

    private static DidDocPeerDid BuildDidDocNumalgo2(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        var keys = peerDid.Value.Substring(11);
        var encodedServices = new List<string>();
        var authentications = new List<VerificationMethodPeerDid>();
        var keyAgreement = new List<VerificationMethodPeerDid>();
        var keyCounter = 1;

        foreach (var part in keys.Split('.'))
        {
            if (string.IsNullOrEmpty(part)) continue;
        
            var prefix = part[0];
            var value = part.Substring(1);

            // Convert the character to the corresponding enum value
            if (prefix == 'S')  // SERVICE
            {
                encodedServices.Add(value);
            }
            else if (prefix == 'V')  // AUTHENTICATION
            {
                var decodedAuthEncumbasis = DecodeMultibaseEncnumbasisAuth(value, format);
                var verMethod = new VerificationMethodPeerDid
                {
                    Id = $"#key-{keyCounter + 1}",
                    Controller = peerDid.Value,
                    VerMaterial = decodedAuthEncumbasis.VerMaterial
                };
                authentications.Add(verMethod);
                keyCounter++;
            }
            else if (prefix == 'E')  // KEY_AGREEMENT
            {
                var decodedAgreemEncumbasis = DecodeMultibaseEncnumbasisAgreement(value, format);
                keyAgreement.Add(new VerificationMethodPeerDid
                {
                    Id = "#key-1",
                    Controller = peerDid.Value,
                    VerMaterial = decodedAgreemEncumbasis.VerMaterial
                });
            }
            else
            {
                throw new ArgumentException($"Unsupported transform part of PeerDID: {prefix}");
            }
        }

        var decodedService = PeerDidHelper.DecodeService(encodedServices, peerDid);
        return new DidDocPeerDid(
            did: peerDid.Value,
            authentications: authentications,
            keyAgreements: keyAgreement,
            services: decodedService
        );
    }
    private static DecodedEncumbasis DecodeMultibaseEncnumbasisAuth(
        string multibase,
        VerificationMaterialFormatPeerDid format
    )
    {
        try
        {
            DecodedEncumbasis decodedEncumbasis = PeerDidHelper.DecodeMultibaseEncnumbasis(multibase, format);
            Validation.ValidateAuthenticationMaterialType(decodedEncumbasis.VerMaterial);
            return decodedEncumbasis;
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDidException($"Invalid key {multibase}", e);
        }
    }

    private static DecodedEncumbasis DecodeMultibaseEncnumbasisAgreement(
        string multibase,
        VerificationMaterialFormatPeerDid format
    )
    {
        try
        {
            DecodedEncumbasis decodedEncumbasis = PeerDidHelper.DecodeMultibaseEncnumbasis(multibase, format);
            Validation.ValidateAgreementMaterialType(decodedEncumbasis.VerMaterial);
            return decodedEncumbasis;
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDidException($"Invalid key {multibase}", e);
        }
    }
}