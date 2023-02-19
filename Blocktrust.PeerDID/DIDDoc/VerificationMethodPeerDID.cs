namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json.Serialization;
using Common.Models.DidDoc;
using Types;

public class VerificationMethodPeerDid
{
    [JsonPropertyName(DidDocConstants.Id)] public string Id { get; set; }
    [JsonPropertyName(DidDocConstants.Controller)] public string Controller { get; set; }

    public VerificationMaterialPeerDid<VerificationMethodTypePeerDid> VerMaterial { get; set; } 

    private string PublicKeyField()
    {
        switch (VerMaterial.Format)
        {
            case VerificationMaterialFormatPeerDid.Base58:
                return PublicKeyFieldValues.Base58;
            case VerificationMaterialFormatPeerDid.Jwk:
                return PublicKeyFieldValues.Jwk;
            case VerificationMaterialFormatPeerDid.Multibase:
                return PublicKeyFieldValues.Multibase;
        }

        return null;
    }

    public Dictionary<string, object> ToDict()
    {
        var dict = new Dictionary<string, object>();
        dict.Add(DidDocConstants.Id, Id);
        dict.Add(DidDocConstants.Type, VerMaterial.Type.Value);
        dict.Add(DidDocConstants.Controller, Controller);
        if (VerMaterial.Format == VerificationMaterialFormatPeerDid.Jwk)
        {
            dict.Add(PublicKeyField(), VerMaterial.ValueAsDictionaryStringString());
        }
        else if (VerMaterial.Format == VerificationMaterialFormatPeerDid.Base58 || VerMaterial.Format == VerificationMaterialFormatPeerDid.Multibase)
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