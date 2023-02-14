namespace Blocktrust.PeerDID.Core;

using System.Text.Json;
using Types;

public static class Validation
{
    internal static void ValidateAuthenticationMaterialType(VerificationMaterialPeerDid<VerificationMethodTypePeerDid> verificationMaterial)
    {
        if (!(verificationMaterial.Type is VerificationMethodTypeAuthentication))
            throw new System.ArgumentException("Invalid verification material type: " + verificationMaterial.Type + " instead of VerificationMaterialAuthentication");
    }

    internal static void ValidateAgreementMaterialType(VerificationMaterialPeerDid<VerificationMethodTypePeerDid> verificationMaterial)
    {
        if (!(verificationMaterial.Type is VerificationMethodTypeAgreement))
            throw new System.ArgumentException("Invalid verification material type: " + verificationMaterial.Type + " instead of VerificationMaterialAgreement");
    }

    public static void ValidateJson(string value)
    {
        try
        {
            JsonSerializer.Deserialize<object>(value);
        }
        catch (Exception ex)
        {
            throw new System.ArgumentException("Invalid JSON " + value, ex);
        }
    }

    internal static void ValidateRawKeyLength(byte[] key)
    {
// for all supported key types now (ED25519 and X25510) the expected size is 32
        if (key.Length != 32)
        {
            throw new ArgumentException("Invalid key " + key);
        }
    }
}