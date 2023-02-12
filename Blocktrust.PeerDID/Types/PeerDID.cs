namespace Blocktrust.PeerDID.Types;

public class PeerDid
{
    public string Value { get; }

    public PeerDid(string value)
    {
        Value = value;
    }
}