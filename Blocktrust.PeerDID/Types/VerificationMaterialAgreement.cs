namespace Blocktrust.PeerDID.Types;

public class VerificationMaterialAgreement : VerificationMaterialPeerDid<VerificationMethodTypePeerDid>
{
    public VerificationMaterialAgreement(VerificationMaterialFormatPeerDid format, object value, VerificationMethodTypeAgreement type) : base(format, value, type)
    {
    }
}