namespace Blocktrust.PeerDID.Types;

//TODO change correct? 
// public class VerificationMaterialAuthentication : VerificationMaterialPeerDID<VerificationMethodTypeAuthentication>
public class VerificationMaterialAuthentication : VerificationMaterialPeerDid<VerificationMethodTypePeerDid>
{
    public VerificationMaterialAuthentication(VerificationMaterialFormatPeerDid format, object value, VerificationMethodTypeAuthentication type) : base(format, value, type)
    {
    }
}