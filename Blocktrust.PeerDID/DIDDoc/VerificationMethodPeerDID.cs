namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json.Serialization;
using Types;

public class VerificationMethodPeerDid
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("controller")] public string Controller { get; set; }

    public VerificationMaterialPeerDid<VerificationMethodTypePeerDid> VerMaterial { get; set; } 

    private string PublicKeyField()
    {
        switch (VerMaterial.Format)
        {
            case VerificationMaterialFormatPeerDid.BASE58:
                return PublicKeyFieldValues.BASE58;
            case VerificationMaterialFormatPeerDid.JWK:
                return PublicKeyFieldValues.JWK;
            case VerificationMaterialFormatPeerDid.MULTIBASE:
                return PublicKeyFieldValues.MULTIBASE;
        }

        return null;
    }

    public Dictionary<string, object> ToDict()
    {
        var dict = new Dictionary<string, object>();
        dict.Add("id", Id);
        dict.Add("type", VerMaterial.Type.Value);
        dict.Add("controller", Controller);
        if (VerMaterial.Format == VerificationMaterialFormatPeerDid.JWK)
        {
            dict.Add(PublicKeyField(), VerMaterial.ValueAsDictionaryStringString());
        }
        else if (VerMaterial.Format == VerificationMaterialFormatPeerDid.BASE58 || VerMaterial.Format == VerificationMaterialFormatPeerDid.MULTIBASE)
        {
            dict.Add(PublicKeyField(), (string)VerMaterial.Value);
        }
        else
        {
            throw new ArgumentException("Unknown format");
        }
        return dict;
    }
}