namespace Blocktrust.PeerDID.Core;

using System.Text.Json;
using Types;

public static class JWK_OKP
{
    internal static Dictionary<string, string> ToJwk(byte[] publicKey, VerificationMethodTypePeerDID verMethodType)
    {
        string x = System.Convert.ToBase64String(publicKey);
        string crv;
        if (verMethodType.Value.Equals(VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020.Value))
        {
            crv = "Ed25519";
        }
        else if (verMethodType.Value.Equals(VerificationMethodTypeAgreement.JSON_WEB_KEY_2020.Value))
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

    //TODO check if this is correct 
    internal static byte[] FromJwk(VerificationMaterialPeerDID<VerificationMethodTypePeerDID> verMaterial)
    {
        Dictionary<string, string> jwkDict = verMaterial.Value as Dictionary<string, string>;
        if (jwkDict == null)
        {
            var jwk = JsonSerializer.Deserialize<Dictionary<string, string>>(verMaterial.Value.ToString());
            jwkDict = jwk;
        }

        if (!jwkDict.ContainsKey("crv"))
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
        return System.Convert.FromBase64String(value);
    }
}