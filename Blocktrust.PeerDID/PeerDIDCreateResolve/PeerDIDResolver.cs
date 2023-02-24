namespace Blocktrust.PeerDID.PeerDIDCreateResolve;

using Blocktrust.PeerDID.Exceptions;
using Common.Models.DidDoc;
using Core;
using DIDDoc;
using FluentResults;
using Types;

public static class PeerDidResolver
{
    /// <summary>
    /// Resolves [DIDDocPeerDID] from [PeerDID]
    /// </summary>
    /// <param name="peerDid">[peerDID] PeerDID to resolve</param>
    /// <param name="format">[format] The format of public keys in the DID DOC. Default format is multibase.</param>
    /// <returns>resolved [DIDDocPeerDID] as JSON string</returns>
    /// <exception cref="MalformedPeerDidException">if [peerDID] parameter does not match [peerDID] spec, if a valid DIDDoc cannot be produced from the [peerDID] </exception>
    /// <exception cref="ArgumentException"></exception>
    public static Result<string> ResolvePeerDid(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        if (!PeerDidCreator.IsPeerDid(peerDid.Value))
        {
            return Result.Fail($"Does not match peer DID regexp: {peerDid}");
        }

        var didDoc = default(DidDocPeerDid);
        if (peerDid.Value[9] == '0')
        {
            try
            {
                didDoc = BuildDidDocNumalgo0(peerDid, format);
            }
            catch (System.Exception e)
            {
                return Result.Fail("Error resolving Peer DID: " + e.Message);
            }
        }
        else if (peerDid.Value[9] == '2')
        {
            try
            {
                didDoc = BuildDidDocNumalgo2(peerDid, format);
            }
            catch (System.Exception e)
            {
                return Result.Fail("Error resolving Peer DID: " + e.Message);
            }
        }
        else
        {
            return Result.Fail($"Invalid numalgo of Peer DID: {peerDid}");
        }

        return Result.Ok(didDoc.ToJson());
    }

    private static DidDocPeerDid BuildDidDocNumalgo0(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        var inceptionKey = peerDid.Value.Substring(10);
        var decodedEncumbasis = DecodeMultibaseEncnumbasisAuth(inceptionKey, format);
        return new DidDocPeerDid(
            did: peerDid.Value,
            authentications: new List<VerificationMethodPeerDid> { PeerDidHelper.GetVerificationMethod(peerDid.Value, decodedEncumbasis) });
    }

    private static DidDocPeerDid BuildDidDocNumalgo2(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        var keys = peerDid.Value.Substring(11);

        var service = "";
        var authentications = new List<VerificationMethodPeerDid>();
        var keyAgreement = new List<VerificationMethodPeerDid>();

        foreach (var part in keys.Split('.'))
        {
            var prefix = part[0];
            var value = part.Substring(1);

            if (prefix.Equals(Numalgo2Prefix.SERVICE))
            {
                service = value;
            }
            else if (prefix.Equals(Numalgo2Prefix.AUTHENTICATION))
            {
                var decodedEncumbasis1 = DecodeMultibaseEncnumbasisAuth(value, format);
                authentications.Add(PeerDidHelper.GetVerificationMethod(peerDid.Value, decodedEncumbasis1));
            }
            else if (prefix.Equals(Numalgo2Prefix.KEY_AGREEMENT))
            {
                var decodedEncumbasis2 = DecodeMultibaseEncnumbasisAgreement(value, format);
                keyAgreement.Add(PeerDidHelper.GetVerificationMethod(peerDid.Value, decodedEncumbasis2));
            }
            else
            {
                throw new System.ArgumentException($"Unsupported transform part of PeerDID: {prefix}");
            }
        }

        var decodedService = DoDecodeService(service, peerDid.Value);

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
            throw new MalformedPeerDidException("Invalid key " + multibase, e);
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
            throw new MalformedPeerDidException("Invalid key " + multibase, e);
        }
    }

    private static List<Service> DoDecodeService(string service, string peerDid)
    {
        try
        {
            return PeerDidHelper.DecodeService(service, new PeerDid(peerDid));
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDidException("Invalid service", e);
        }
    }
}