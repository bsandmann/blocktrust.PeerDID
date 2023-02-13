namespace Blocktrust.PeerDID.Types;

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


    public Dictionary<string,string> ValueAsDictionaryStringString()
    {
        //TODO DRY i used it somehwer else
        // This code could need improvement: it is not clear what is the expected input is. It could be a string, a dictionary (s,s) or a dictionary (s,o)
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

        return jwkDict;
    }
}