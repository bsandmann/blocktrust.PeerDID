namespace Blocktrust.PeerDID.Core;

using System.Text.Json;

public class Utils
{
    public static string ToJson(object value)
    {
        return JsonSerializer.Serialize(value);
    }

    public static List<Dictionary<string, object>> FromJsonToList(string value)
    {
        return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(value);
    }

    public static Dictionary<string, object> FromJsonToMap(string value)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(value);
    } 
}