namespace Blocktrust.PeerDID.Types;

public abstract class VerificationMethodTypePeerDid
{
    public string Value { get; }

    protected VerificationMethodTypePeerDid(string value)
    {
        Value = value;
    }
}