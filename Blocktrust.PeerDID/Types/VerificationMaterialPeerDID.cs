namespace Blocktrust.PeerDID.Types;

public class VerificationMaterialPeerDid<T> where T : VerificationMethodTypePeerDid
{
    public VerificationMaterialFormatPeerDid Format { get; }
    public object Value { get; }
    public T Type { get; }

    public VerificationMaterialPeerDid(VerificationMaterialFormatPeerDid format, object value, T type)
    {
        Format = format;
        Value = value;
        Type = type;
    }
}