namespace Blocktrust.PeerDID.Types;

public class VerificationMethodTypeAuthentication : VerificationMethodTypePeerDid
{
    public static readonly VerificationMethodTypeAuthentication JSON_WEB_KEY_2020 = new VerificationMethodTypeAuthentication("JsonWebKey2020");
    public static readonly VerificationMethodTypeAuthentication ED25519_VERIFICATION_KEY_2018 = new VerificationMethodTypeAuthentication("Ed25519VerificationKey2018");
    public static readonly VerificationMethodTypeAuthentication ED25519_VERIFICATION_KEY_2020 = new VerificationMethodTypeAuthentication("Ed25519VerificationKey2020");

    private VerificationMethodTypeAuthentication(string value) : base(value)
    {
    }
}