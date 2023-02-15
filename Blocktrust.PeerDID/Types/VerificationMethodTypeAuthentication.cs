namespace Blocktrust.PeerDID.Types;

public class VerificationMethodTypeAuthentication : VerificationMethodTypePeerDid
{
    public static readonly VerificationMethodTypeAuthentication JsonWebKey2020 = new VerificationMethodTypeAuthentication("JsonWebKey2020");
    public static readonly VerificationMethodTypeAuthentication Ed25519VerificationKey2018 = new VerificationMethodTypeAuthentication("Ed25519VerificationKey2018");
    public static readonly VerificationMethodTypeAuthentication Ed25519VerificationKey2020 = new VerificationMethodTypeAuthentication("Ed25519VerificationKey2020");

    private VerificationMethodTypeAuthentication(string value) : base(value)
    {
    }
}