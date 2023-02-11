namespace Blocktrust.PeerDID.DIDDoc;

using Types;

public class VerificationMethodPeerDID
{
    public string Id { get; set; }
    public string Controller { get; set; }
    public VerificationMaterialPeerDID<VerificationMethodTypePeerDID> VerMaterial { get; set; }

    private string PublicKeyField()
    {
        switch (VerMaterial.Format)
        {
            case VerificationMaterialFormatPeerDID.BASE58:
                return PublicKeyFieldValues.BASE58;
            case VerificationMaterialFormatPeerDID.JWK:
                return PublicKeyFieldValues.JWK;
            case VerificationMaterialFormatPeerDID.MULTIBASE:
                return PublicKeyFieldValues.MULTIBASE;
        }

        return null;
    }

    public Dictionary<string, string> ToDict()
    {
        var dict = new Dictionary<string, string>();
        dict.Add("id", Id);
        dict.Add("type", VerMaterial.Type.Value);
        dict.Add("controller", Controller);
        dict.Add(PublicKeyField(), (string)VerMaterial.Value);
        return dict;
    }
}