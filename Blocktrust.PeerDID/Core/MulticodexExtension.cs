namespace Blocktrust.PeerDID.Core;

using Types;

public static class MulticodexExtension
{
    public static Multicodec GetCodec(VerificationMethodTypePeerDid keyType)
    {
        switch (keyType)
        {
            case VerificationMethodTypeAuthentication:
                return Multicodec.Ed25519Public;
            case VerificationMethodTypeAgreement:
                return Multicodec.X25519Public;
            default:
                return default;
        }
    }
}