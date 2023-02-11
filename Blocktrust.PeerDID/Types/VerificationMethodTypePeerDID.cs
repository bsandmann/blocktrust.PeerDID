namespace Blocktrust.PeerDID.Types;

public abstract class VerificationMethodTypePeerDID
{
    public string Value { get; }

    protected VerificationMethodTypePeerDID(string value)
    {
        Value = value;
    }
}