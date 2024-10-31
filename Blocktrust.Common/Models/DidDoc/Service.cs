using System.Text.Json.Serialization;

namespace Blocktrust.Common.Models.DidDoc;

public class ServiceEndpoint
{
    public string Uri { get; set; }
    public List<string>? RoutingKeys { get; set; }
    public List<string>? Accept { get; set; }
    
    public ServiceEndpoint(string uri, List<string>? routingKeys = null, List<string>? accept = null)
    {
        Uri = uri;
        RoutingKeys = routingKeys;
        Accept = accept;
    }
}

public class Service
{
    public string Id { get; }
    public string Type { get; }
    
    [JsonConverter(typeof(ServiceEndpointConverter))]
    public ServiceEndpoint ServiceEndpoint { get; }

    public Service(string id, ServiceEndpoint serviceEndpoint, string type = ServiceConstants.ServiceDidcommMessaging)
    {
        Id = id;
        Type = type;
        ServiceEndpoint = serviceEndpoint;
    }

    public Dictionary<string, object> ToDict()
    {
        var res = new Dictionary<string, object>
        {
            { ServiceConstants.ServiceId, Id },
            { ServiceConstants.ServiceType, Type },
            { ServiceConstants.ServiceEndpoint, new Dictionary<string, object>
                {
                    { "uri", ServiceEndpoint.Uri },
                    { "routingKeys", ServiceEndpoint.RoutingKeys ?? new List<string>() },
                    { "accept", ServiceEndpoint.Accept ?? new List<string>() }
                }
            }
        };
        return res;
    }

    public static Service FromDictionary(Dictionary<string, object> dict)
    {
        var id = string.Empty;
        var type = string.Empty;
        var serviceEndpoint = new ServiceEndpoint(string.Empty);

        if (dict.ContainsKey(ServiceConstants.ServiceId))
            id = dict[ServiceConstants.ServiceId].ToString();

        if (dict.ContainsKey(ServiceConstants.ServiceType))
            type = dict[ServiceConstants.ServiceType].ToString();

        if (dict.ContainsKey(ServiceConstants.ServiceEndpoint))
        {
            var endpoint = dict[ServiceConstants.ServiceEndpoint];
            if (endpoint is Dictionary<string, object> endpointDict)
            {
                serviceEndpoint = new ServiceEndpoint(
                    endpointDict.GetValueOrDefault("uri", string.Empty)?.ToString() ?? string.Empty,
                    endpointDict.GetValueOrDefault("routingKeys") as List<string>,
                    endpointDict.GetValueOrDefault("accept") as List<string>
                );
            }
        }

        return new Service(id, serviceEndpoint, type);
    }
}