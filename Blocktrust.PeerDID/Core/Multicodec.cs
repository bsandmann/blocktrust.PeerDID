namespace Blocktrust.PeerDID.Core;

using Types;

public class Multicodec
{
    public const string NameX25519 = "X25519";
    public const string NameED25519 = "ED25519";
    public static readonly Multicodec X25519Public = new Multicodec(NameX25519, new byte[] { 0xEC }, 236);
    public static readonly Multicodec Ed25519Public = new Multicodec(NameED25519, new byte[] { 0xED }, 237);
    public static readonly Multicodec X25519Private = new Multicodec(NameX25519, new byte[] { 0x13, 0x00 }, 4864);
    public static readonly Multicodec Ed25519Private = new Multicodec(NameED25519, new byte[] { 0x13, 0x02 }, 4866);

    public string Name { get; }
    public byte[] PrefixByte { get; }
    public int PrefixInt { get; }

    public Multicodec(string name, byte[] prefixByte, int prefixInt)
    {
        Name = name;
        PrefixByte = prefixByte;
        PrefixInt = prefixInt;
    }

    public static byte[] ToMulticodec(byte[] value, int prefix)
    // public static byte[] ToMulticodec(byte[] value, VerificationMethodTypePeerDid keyType)
    {
        // int prefix = GetCodec(keyType).PrefixInt;
        var byteBuffer = new byte[2];
        MemoryStream stream = new MemoryStream(byteBuffer);
        VarInt.WriteVarInt(prefix, stream);
        return byteBuffer.Concat(value).ToArray();
    }

    public static KeyValuePair<Multicodec, byte[]> FromMulticodec(byte[] value)
    {
        int prefix = VarInt.ReadVarInt(new MemoryStream(value));
        Multicodec codec = GetCodec(prefix);
        var byteBuffer = new byte[2];
        MemoryStream stream = new MemoryStream(byteBuffer);
        VarInt.WriteVarInt(prefix, stream);
        return new KeyValuePair<Multicodec, byte[]>(codec, value.Skip(byteBuffer.Length).ToArray());
    }

    private static Multicodec GetCodec(VerificationMethodTypePeerDid keyType)
    {
        switch (keyType)
        {
            case VerificationMethodTypeAuthentication:
                return Ed25519Public;
            case VerificationMethodTypeAgreement:
                return X25519Public;
            default:
                return default;
        }
    }

    private static Multicodec GetCodec(int prefix)
    {
        if (prefix == X25519Public.PrefixInt)
        {
            return X25519Public;
        }
        else if (prefix == Ed25519Public.PrefixInt)
        {
            return Ed25519Public;
        }
        else if (prefix == Ed25519Private.PrefixInt)
        {
            return Ed25519Private;
        }
        else if (prefix == X25519Private.PrefixInt)
        {
            return X25519Private;
        }
        else
        {
            throw new ArgumentException("Invalid key: Prefix " + prefix + " not supported");
        }
    }
}