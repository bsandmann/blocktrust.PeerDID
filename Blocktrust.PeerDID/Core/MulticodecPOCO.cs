namespace Blocktrust.PeerDID.Core;

public class MulticodecPoco
{
    public string Name {get; set;}
    public byte[] PrefixByte {get; set;}
    public int PrefixInt { get; set; }

    public MulticodecPoco(string name, byte[] prefixByte, int prefixInt)
    {
        Name = name;
        PrefixByte = prefixByte;
        PrefixInt = prefixInt;
    }
}