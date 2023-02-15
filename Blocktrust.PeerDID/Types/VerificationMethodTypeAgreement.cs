namespace Blocktrust.PeerDID.Types;

public class VerificationMethodTypeAgreement : VerificationMethodTypePeerDid
{
    public static readonly VerificationMethodTypeAgreement JsonWebKey2020 = new VerificationMethodTypeAgreement("JsonWebKey2020");
    public static readonly VerificationMethodTypeAgreement X25519KeyAgreementKey2019 = new VerificationMethodTypeAgreement("X25519KeyAgreementKey2019");
    public static readonly VerificationMethodTypeAgreement X25519KeyAgreementKey2020 = new VerificationMethodTypeAgreement("X25519KeyAgreementKey2020");

    private VerificationMethodTypeAgreement(string value) : base(value)
    {
    }
}