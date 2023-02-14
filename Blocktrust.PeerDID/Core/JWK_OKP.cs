namespace Blocktrust.PeerDID.Core;

using System.Text.Json;
using Common.Converter;
using Types;

public static class JwkOkp
{
    public static Dictionary<string, string> ToJwk(byte[] publicKey, VerificationMethodTypePeerDid verMethodType)
    {
        string x = Base64Url.Encode(publicKey);
        string crv;
        if (verMethodType is VerificationMethodTypeAuthentication)
        {
            crv = "Ed25519";
        }
        else if (verMethodType is VerificationMethodTypeAgreement)
        {
            crv = "X25519";
        }
        else
        {
            throw new System.ArgumentException("Unsupported JWK type " + verMethodType.Value);
        }

        Dictionary<string, string> jwk = new Dictionary<string, string>
        {
            { "kty", "OKP" },
            { "crv", crv },
            { "x", x },
        };
        return jwk;
    }

    public static byte[] FromJwk(VerificationMaterialPeerDid<VerificationMethodTypePeerDid> verMaterial)
    {
        var jwkDict = verMaterial.ValueAsDictionaryStringString();
        if (jwkDict is null)
        {
            var jwk = JsonSerializer.Deserialize<Dictionary<string, string>>((string)verMaterial.Value);
            jwkDict = jwk;
        }

        // ReSharper disable once NullableWarningSuppressionIsUsed
        if (!jwkDict!.ContainsKey("crv"))
        {
            throw new System.ArgumentException("Invalid JWK key - no 'crv' fields: " + verMaterial.Value);
        }

        if (!jwkDict.ContainsKey("x"))
        {
            throw new System.ArgumentException("Invalid JWK key - no 'x' fields: " + verMaterial.Value);
        }

        string crv = jwkDict["crv"];
        if (verMaterial.Type is VerificationMethodTypeAuthentication && crv != "Ed25519")
        {
            throw new System.ArgumentException("Invalid JWK key type - authentication expected: " + verMaterial.Value);
        }

        if (verMaterial.Type is VerificationMethodTypeAgreement && crv != "X25519")
        {
            throw new System.ArgumentException("Invalid JWK key type - key agreement expected: " + verMaterial.Value);
        }

        string value = jwkDict["x"];

        return Base64Url.Decode(value);
    }
}