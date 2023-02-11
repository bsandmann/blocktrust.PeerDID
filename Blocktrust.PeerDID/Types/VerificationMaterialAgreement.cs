namespace Blocktrust.PeerDID.Types;

//TODO change correct? 
// public class VerificationMaterialAgreement : VerificationMaterialPeerDID<VerificationMethodTypeAgreement>
public class VerificationMaterialAgreement : VerificationMaterialPeerDID<VerificationMethodTypePeerDID>
{
    public VerificationMaterialAgreement(VerificationMaterialFormatPeerDID format, object value, VerificationMethodTypeAgreement type) : base(format, value, type)
    {
    }
}