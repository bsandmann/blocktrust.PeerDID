namespace Blocktrust.PeerDID.Core;

using System.Text.RegularExpressions;
using SimpleBase;

public class Multibase
{
    enum MultibasePrefix
    {
        BASE58 = 'z'
    }

    public static string ToBase58Multibase(byte[] value)
    {
        //TODO unclear if this simplification is correct
        // return Multibase.Encode(Multibase.Base.Base58BTC, value);
        var f = 'z' + Base58.Bitcoin.Encode(value);
        return f;
    }

    public static string ToBase58(byte[] value)
    {
        // return Base58.Encode(value);
        return Base58.Bitcoin.Encode(value);
    }

    public static (string, byte[]) FromBase58Multibase(string multibase)
    {
        if (string.IsNullOrEmpty(multibase))
        {
            throw new System.ArgumentException("Invalid key: No transform part in multibase encoding");
        }

        char transform = multibase[0];
        if (transform != (char)MultibasePrefix.BASE58)
        {
            throw new System.ArgumentException("Invalid key: Prefix " + transform + " not supported");
        }

        string encnumbasis = multibase.Substring(1);
        byte[] decodedEncnumbasis = FromBase58(encnumbasis);
        return (encnumbasis, decodedEncnumbasis);
    }

    public static byte[] FromBase58(string value)
    {
        if (!IsBase58(value))
        {
            throw new System.ArgumentException("Invalid key: Invalid base58 encoding: " + value);
        }

        return Base58.Bitcoin.Decode(value);
        // return Base58.Decode(value);
    }

    public static bool IsBase58(string value)
    {
        Regex alphabet = new Regex("[1-9a-km-zA-HJ-NP-Z]+");
        return alphabet.IsMatch(value);
    }
}