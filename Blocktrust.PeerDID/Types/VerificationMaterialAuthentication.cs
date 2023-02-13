namespace Blocktrust.PeerDID.Types;

public class VerificationMaterialAuthentication : VerificationMaterialPeerDid<VerificationMethodTypePeerDid>
{
    public VerificationMaterialAuthentication(VerificationMaterialFormatPeerDid format, object value, VerificationMethodTypeAuthentication type) : base(format, value, type)
    {
    }
}