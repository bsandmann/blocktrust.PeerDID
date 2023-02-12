namespace Blocktrust.PeerDID.DIDDoc;

using Types;

public class VerificationMethodPeerDid
{
    public string Id { get; set; }
    public string Controller { get; set; }
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