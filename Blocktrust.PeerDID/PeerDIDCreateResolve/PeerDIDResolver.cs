namespace Blocktrust.PeerDID.PeerDIDCreateResolve;

using Blocktrust.PeerDID.Exceptions;
using Core;
using DIDDoc;
using Types;

public static class PeerDIDResolver
{
    /// <summary>
    /// Resolves [DIDDocPeerDID] from [PeerDID]
    /// </summary>
    /// <param name="peerDID">[peerDID] PeerDID to resolve</param>
    /// <param name="format">[format] The format of public keys in the DID DOC. Default format is multibase.</param>
    /// <returns>resolved [DIDDocPeerDID] as JSON string</returns>
    /// <exception cref="MalformedPeerDIDException">if [peerDID] parameter does not match [peerDID] spec, if a valid DIDDoc cannot be produced from the [peerDID] </exception>
    /// <exception cref="ArgumentException"></exception>
    public static string ResolvePeerDID(PeerDID peerDID, VerificationMaterialFormatPeerDID format = VerificationMaterialFormatPeerDID.MULTIBASE)
    {
        if (!PeerDIDCreator.IsPeerDID(peerDID.Value))
        {
            throw new MalformedPeerDIDException($"Does not match peer DID regexp: {peerDID}");
        }

        var didDoc = default(DIDDocPeerDID);
        if (peerDID.Value[9] == '0')
        {
            didDoc = BuildDIDDocNumalgo0(peerDID, format);
        }
        else if (peerDID.Value[9] == '2')
        {
            didDoc = BuildDIDDocNumalgo2(peerDID, format);
        }
        else
        {
            throw new System.ArgumentException($"Invalid numalgo of Peer DID: {peerDID}");
        }

        return didDoc.toJson();
    }

    private static DIDDocPeerDID BuildDIDDocNumalgo0(PeerDID peerDID, VerificationMaterialFormatPeerDID format)
    {
        var inceptionKey = peerDID.Value.Substring(10);
        var decodedEncumbasis = DecodeMultibaseEncnumbasisAuth(inceptionKey, format);
        return new DIDDocPeerDID(
            did: peerDID.Value,
            authentication: new List<VerificationMethodPeerDID> { PeerDIDHelper.GetVerificationMethod(peerDID.Value, decodedEncumbasis) });
    }

    private static DIDDocPeerDID BuildDIDDocNumalgo2(PeerDID peerDID, VerificationMaterialFormatPeerDID format)
    {
        var keys = peerDID.Value.Substring(11);

        var service = "";
        var authentications = new List<VerificationMethodPeerDID>();
        var keyAgreement = new List<VerificationMethodPeerDID>();

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
                authentications.Add(PeerDIDHelper.GetVerificationMethod(peerDID.Value, decodedEncumbasis1));
            }
            else if (prefix.Equals(Numalgo2Prefix.KEY_AGREEMENT))
            {
                var decodedEncumbasis2 = DecodeMultibaseEncnumbasisAgreement(value, format);
                keyAgreement.Add(PeerDIDHelper.GetVerificationMethod(peerDID.Value, decodedEncumbasis2));
            }
            else
            {
                throw new System.ArgumentException($"Unsupported transform part of PeerDID: {prefix}");
            }
        }

        var decodedService = DoDecodeService(service, peerDID.Value);

        return new DIDDocPeerDID(
            did: peerDID.Value,
            authentication: authentications,
            keyAgreement: keyAgreement,
            service: decodedService
        );
    }

    private static DecodedEncumbasis DecodeMultibaseEncnumbasisAuth(
        string multibase,
        VerificationMaterialFormatPeerDID format
    )
    {
        try
        {
            DecodedEncumbasis decodedEncumbasis = PeerDIDHelper.DecodeMultibaseEncnumbasis(multibase, format);
            //TODO correct? 
            if (decodedEncumbasis.VerMaterial.Type is VerificationMethodTypeAgreement)
            {
                Validation.ValidateAuthenticationMaterialType(decodedEncumbasis.VerMaterial);
            }
            else if (decodedEncumbasis.VerMaterial.Type is VerificationMethodTypeAgreement)
            {
                Validation.ValidateAuthenticationMaterialType(decodedEncumbasis.VerMaterial);
            }
            else
            {
                throw new Exception();
            }

            return decodedEncumbasis;
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDIDException("Invalid key " + multibase, e);
        }
    }

    private static DecodedEncumbasis DecodeMultibaseEncnumbasisAgreement(
        string multibase,
        VerificationMaterialFormatPeerDID format
    )
    {
        try
        {
            DecodedEncumbasis decodedEncumbasis = PeerDIDHelper.DecodeMultibaseEncnumbasis(multibase, format);
            Validation.ValidateAgreementMaterialType(decodedEncumbasis.VerMaterial);
            return decodedEncumbasis;
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDIDException("Invalid key " + multibase, e);
        }
    }

    private static List<Service> DoDecodeService(string service, string peerDID)
    {
        try
        {
            return PeerDIDHelper.DecodeService(service, new PeerDID(peerDID));
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDIDException("Invalid service", e);
        }
    }
}