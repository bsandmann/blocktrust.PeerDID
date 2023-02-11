namespace Blocktrust.PeerDID.Core;


//TODO identical to DIDComm
public class VarInt
{
    //TODO this code was translated from Java, because we are missing the library for VarInt in .NET
    //no clue if the translation is correct
    public static void WriteVarInt(int value, MemoryStream stream)
    {
        while ((value & 0xFFFFFF80) != 0L)
        {
            stream.WriteByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }

        stream.WriteByte((byte)(value & 0x7F));
    }

    public static int ReadVarInt(MemoryStream stream)
    {
        int value = 0;
        int i = 0;
        int b = 0;
        while (stream.Length > stream.Position && ((b = stream.ReadByte()) & 0x80) != 0)
        {
            value |= (b & 0x7F) << i;
            i += 7;
            if (i > 35)
            {
                throw new ArgumentException("Variable length quantity is too long");
            }
        }

        value = value | (b << i);
        return value;
    }
}
