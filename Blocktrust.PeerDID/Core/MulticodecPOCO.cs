namespace Blocktrust.PeerDID.Core;

public class MulticodecPoco
{
    public string Name {get; }
    public byte[] PrefixByte {get; }
    public int PrefixInt { get; }

    public MulticodecPoco(string name, byte[] prefixByte, int prefixInt)
    {
        Name = name;
        PrefixByte = prefixByte;
        PrefixInt = prefixInt;
    }
}