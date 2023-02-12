namespace Blocktrust.PeerDID.Core;

using Types;

public static class Multicodec
{
   public const string NameX25519 = "X25519";
   public const string NameED25519 = "ED25519";
   public static readonly MulticodecPOCO X25519 = new MulticodecPOCO(NameX25519, new byte[] { 0xEC }, 236);
   public static readonly MulticodecPOCO Ed25519 = new MulticodecPOCO(NameED25519, new byte[] { 0xED }, 237);
    
    // public enum Codec
    // {
    //     X25519 = 0xEC,
    //     ED25519 = 0xED
    // }

    public static byte[] ToMulticodec(byte[] value, VerificationMethodTypePeerDID keyType)
    {
        int prefix = GetCodec(keyType).PrefixInt;
        var byteBuffer = new byte[2];
        //TODO unclear if the implementation is correct
        MemoryStream stream = new MemoryStream(byteBuffer);
        VarInt.WriteVarInt(prefix, stream);
        return byteBuffer.Concat(value).ToArray();
    }

    public static KeyValuePair<MulticodecPOCO, byte[]> FromMulticodec(byte[] value)
    {
        int prefix = VarInt.ReadVarInt(new MemoryStream(value));
        MulticodecPOCO codec = GetCodec(prefix);
        var byteBuffer = new byte[2];
        //TODO unclear if the implementation is correct
        MemoryStream stream = new MemoryStream(byteBuffer);
        VarInt.WriteVarInt(prefix, stream);
        return new KeyValuePair<MulticodecPOCO, byte[]>(codec, value.Skip(byteBuffer.Length).ToArray());
    }

    private static MulticodecPOCO GetCodec(VerificationMethodTypePeerDID keyType)
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

    private static MulticodecPOCO GetCodec(int prefix)
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