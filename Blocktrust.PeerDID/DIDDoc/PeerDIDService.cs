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
            { ServiceConstants.ServiceId, Id },
            { ServiceConstants.ServiceType, Type }
        };
        res.Add(ServiceConstants.ServiceEndpoint, ServiceEndpoint);
        if (RoutingKeys is not null)
        {
            res.Add(ServiceConstants.ServiceRoutingKeys, RoutingKeys);
        }

        if (Accept is not null)
        {
            res.Add(ServiceConstants.ServiceAccept, Accept);
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
        if (dict.ContainsKey(ServiceConstants.ServiceId))
        {
            id = dict[ServiceConstants.ServiceId].ToString();
        }

        if (dict.ContainsKey(ServiceConstants.ServiceType))
        {
            type = dict[ServiceConstants.ServiceType].ToString();
        }

        if (dict.ContainsKey(ServiceConstants.ServiceEndpoint))
        {
            serviceEndpoint = dict[ServiceConstants.ServiceEndpoint].ToString();
        }

        if (dict.ContainsKey(ServiceConstants.ServiceRoutingKeys))
        {
            if (dict[ServiceConstants.ServiceRoutingKeys] is JsonElement)
            {
                var routingKeysJsonElement = (JsonElement)dict[ServiceConstants.ServiceRoutingKeys];
                routingKeys = routingKeysJsonElement!.EnumerateArray().Select(p => p.GetString()).ToList();
            }
            else if (dict[ServiceConstants.ServiceRoutingKeys] is List<string>)
            {
                routingKeys = dict[ServiceConstants.ServiceRoutingKeys] as List<string>;
            }
            else
            {
                throw new ArgumentException("Unknown data format");
            }
        }

        if (dict.ContainsKey(ServiceConstants.ServiceAccept))
        {
            if (dict[ServiceConstants.ServiceAccept] is JsonElement)
            {
                var acceptJsonElement = (JsonElement)dict[ServiceConstants.ServiceAccept];
                accept = acceptJsonElement!.EnumerateArray().Select(p => p.GetString()).ToList();
            }
            else if (dict[ServiceConstants.ServiceAccept] is List<string>)
            {
                accept = dict[ServiceConstants.ServiceAccept] as List<string>;
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