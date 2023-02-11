namespace Blocktrust.PeerDID.Types;

public class VerificationMaterialPeerDID<T> where T : VerificationMethodTypePeerDID
{
    public VerificationMaterialFormatPeerDID Format { get; }
    public object Value { get; }
    public T Type { get; }

    public VerificationMaterialPeerDID(VerificationMaterialFormatPeerDID format, object value, T type)
    {
        Format = format;
        Value = value;
        Type = type;
    }
}