namespace Blocktrust.PeerDID.Types;

//TODO change correct? 
// public class VerificationMaterialAgreement : VerificationMaterialPeerDID<VerificationMethodTypeAgreement>
public class VerificationMaterialAgreement : VerificationMaterialPeerDid<VerificationMethodTypePeerDid>
{
    public VerificationMaterialAgreement(VerificationMaterialFormatPeerDid format, object value, VerificationMethodTypeAgreement type) : base(format, value, type)
    {
    }
}