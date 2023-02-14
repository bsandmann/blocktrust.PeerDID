namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json;
using System.Text.Json.Serialization;

public class PeerDidService
{
    public string Id;
    public string Type;
    public string ServiceEndpoint;
    public List<string>? RoutingKeys;
    public List<string>? Accept;

    public PeerDidService(string id, string type, string serviceEndpoint, List<string> routingKeys, List<string> accept)
    {
        this.Id = id;
        this.Type = type;
        this.ServiceEndpoint = serviceEndpoint;
        this.RoutingKeys = routingKeys;
        this.Accept = accept;
    }

    public Dictionary<string, object> ToDict()
    {
        var res = new Dictionary<string, object>
        {
            { "id", Id },
            { "type", Type }
        };
        res.Add("serviceEndpoint", ServiceEndpoint);
        if (RoutingKeys is not null)
        {
            res.Add("routingKeys", RoutingKeys);
        }

        if (Accept is not null)
        {
            res.Add("accept", Accept);
        }

        return res;
    }

    public static PeerDidService FromDictionary(Dictionary<string, object> dict)
    {
        // var service = new PeerDIDService();
        var id = string.Empty;
        var type = string.Empty;
        var serviceEndpoint = string.Empty;
        List<string>? routingKeys = null;
        List<string>? accept = null;
        if (dict.ContainsKey("id"))
        {
            id = dict["id"].ToString();
        }

        if (dict.ContainsKey("type"))
        {
            type = dict["type"].ToString();
        }

        if (dict.ContainsKey("serviceEndpoint"))
        {
            serviceEndpoint = dict["serviceEndpoint"].ToString();
        }

        if (dict.ContainsKey("routingKeys"))
        {
            if (dict["routingKeys"] is JsonElement)
            {
                var routingKeysJsonElement = (JsonElement)dict["routingKeys"];
                routingKeys = routingKeysJsonElement!.EnumerateArray().Select(p => p.GetString()).ToList();
            }
            else if (dict["routingKeys"] is List<string>)
            {
                routingKeys = dict["routingKeys"] as List<string>;
            }
            else
            {
                throw new ArgumentException("Unknown data format");
            }
        }

        if (dict.ContainsKey("accept"))
        {
            if (dict["accept"] is JsonElement)
            {
                var acceptJsonElement = (JsonElement)dict["accept"];
                accept = acceptJsonElement!.EnumerateArray().Select(p => p.GetString()).ToList();
            }
            else if (dict["accept"] is List<string>)
            {
                accept = dict["accept"] as List<string>;
            }
            else
            {
                throw new ArgumentException("Unknown data format");
            }
        }

        return new PeerDidService(
            id: id,
            type: type,
            serviceEndpoint: serviceEndpoint,
            routingKeys: routingKeys,
            accept: accept);
    }
}