namespace Blocktrust.PeerDID.Types;

//TODO change correct? 
// public class VerificationMaterialAuthentication : VerificationMaterialPeerDID<VerificationMethodTypeAuthentication>
public class VerificationMaterialAuthentication : VerificationMaterialPeerDID<VerificationMethodTypePeerDID>
{
    public VerificationMaterialAuthentication(VerificationMaterialFormatPeerDID format, object value, VerificationMethodTypeAuthentication type) : base(format, value, type)
    {
    }
}