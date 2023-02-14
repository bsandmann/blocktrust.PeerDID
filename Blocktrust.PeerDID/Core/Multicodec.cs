namespace Blocktrust.PeerDID.Core;

using Types;

public static class Multicodec
{
   public const string NameX25519 = "X25519";
   public const string NameED25519 = "ED25519";
   private static readonly MulticodecPoco X25519 = new MulticodecPoco(NameX25519, new byte[] { 0xEC }, 236);
   private static readonly MulticodecPoco Ed25519 = new MulticodecPoco(NameED25519, new byte[] { 0xED }, 237);
    
    public static byte[] ToMulticodec(byte[] value, VerificationMethodTypePeerDid keyType)
    {
        int prefix = GetCodec(keyType).PrefixInt;
        var byteBuffer = new byte[2];
        MemoryStream stream = new MemoryStream(byteBuffer);
        VarInt.WriteVarInt(prefix, stream);
        return byteBuffer.Concat(value).ToArray();
    }

    public static KeyValuePair<MulticodecPoco, byte[]> FromMulticodec(byte[] value)
    {
        int prefix = VarInt.ReadVarInt(new MemoryStream(value));
        MulticodecPoco codec = GetCodec(prefix);
        var byteBuffer = new byte[2];
        MemoryStream stream = new MemoryStream(byteBuffer);
        VarInt.WriteVarInt(prefix, stream);
        return new KeyValuePair<MulticodecPoco, byte[]>(codec, value.Skip(byteBuffer.Length).ToArray());
    }

    private static MulticodecPoco GetCodec(VerificationMethodTypePeerDid keyType)
    {
        switch (keyType)
        {
            case VerificationMethodTypeAuthentication:
                return Ed25519;
            case VerificationMethodTypeAgreement:
                return X25519;
            default:
                return default;
        }
    }

    private static MulticodecPoco GetCodec(int prefix)
    {
        // //TODO very simimlar to DIDComm
        if (prefix == X25519.PrefixInt)
        {
            return X25519;
        } 
        else if (prefix == Ed25519.PrefixInt)
        {
            return Ed25519;
        }
        else
        {
            throw new ArgumentException("Invalid key: Prefix " + prefix + " not supported");
        }
    } 
}