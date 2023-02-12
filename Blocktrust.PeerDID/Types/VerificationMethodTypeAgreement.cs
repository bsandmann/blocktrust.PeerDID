namespace Blocktrust.PeerDID.Types;

public class VerificationMethodTypeAgreement : VerificationMethodTypePeerDid
{
    public static readonly VerificationMethodTypeAgreement JSON_WEB_KEY_2020 = new VerificationMethodTypeAgreement("JsonWebKey2020");
    public static readonly VerificationMethodTypeAgreement X25519_KEY_AGREEMENT_KEY_2019 = new VerificationMethodTypeAgreement("X25519KeyAgreementKey2019");
    public static readonly VerificationMethodTypeAgreement X25519_KEY_AGREEMENT_KEY_2020 = new VerificationMethodTypeAgreement("X25519KeyAgreementKey2020");

    private VerificationMethodTypeAgreement(string value) : base(value)
    {
    }
}