namespace Blocktrust.PeerDID.Types;

public class PeerDID
{
    public string Value { get; }

    public PeerDID(string value)
    {
        Value = value;
    }
}