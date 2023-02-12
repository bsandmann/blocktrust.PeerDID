namespace Blocktrust.PeerDID.Core;

using System.Buffers.Text;
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

    internal static byte[] FromJwk(VerificationMaterialPeerDID<VerificationMethodTypePeerDID> verMaterial)
    {
        // This code could need improvement: it is not clear what is the expected input is. It could be a string, a dictionary (s,s) or a dictionary (s,o)
        var isDictionaryStringObject = verMaterial.Value is Dictionary<string, object>;
        var isDictionaryStringString = verMaterial.Value is Dictionary<string, string>;
        Dictionary<string, string>? jwkDict = null;
        if (isDictionaryStringObject)
        {
            jwkDict = ((Dictionary<string, object>)verMaterial.Value)
                .Select(x => new KeyValuePair<string, string>(x.Key, (string)x.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        else if (isDictionaryStringString)
        {
            jwkDict = ((Dictionary<string, string>)verMaterial.Value);
        }

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

        return Common.Converter.Base64Url.Decode(value);
    }

    public static byte[] Decode(string input)
    {
        string s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 0:
                return Convert.FromBase64String(s);
            case 2:
                s += "==";
                goto case 0;
            case 3:
                s += "=";
                goto case 0;
            default:
                throw new ArgumentOutOfRangeException(nameof(input), "Illegal base64url string!");
        }
    }
}