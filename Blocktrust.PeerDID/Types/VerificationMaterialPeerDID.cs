namespace Blocktrust.PeerDID.Types;

using System.Text.Json;
using DIDDoc;

public class VerificationMaterialPeerDid<T> where T : VerificationMethodTypePeerDid
{
    public VerificationMaterialFormatPeerDid Format { get; }
    public object Value { get; }
    public T Type { get; }

    public VerificationMaterialPeerDid(VerificationMaterialFormatPeerDid format, object value, T type)
    {
        Format = format;
        Value = value;
        Type = type;
    }


    public Dictionary<string, string> ValueAsDictionaryStringString()
    {
        var isDictionaryStringObject = Value is Dictionary<string, object>;
        var isDictionaryStringString = Value is Dictionary<string, string>;
        Dictionary<string, string>? jwkDict = null;
        if (isDictionaryStringObject)
        {
            jwkDict = ((Dictionary<string, object>)Value)
                .Select(x => new KeyValuePair<string, string>(x.Key, (string)x.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        else if (isDictionaryStringString)
        {
            jwkDict = ((Dictionary<string, string>)Value);
        }
        else
        {
            jwkDict = JsonSerializer.Deserialize<Dictionary<string, string>>((string)Value);
        }

        return jwkDict;
    }
}