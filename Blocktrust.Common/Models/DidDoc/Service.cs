﻿namespace Blocktrust.Common.Models.DidDoc;

using System.Text.Json;

public class Service
{
    public string Id { get; }
    public string Type { get; }
    public string ServiceEndpoint { get; }
    public List<string>? RoutingKeys { get; }
    public List<string>? Accept { get; }
    
    public Service(string id, string serviceEndpoint, List<string>? routingKeys, List<string> accept, string type = ServiceConstants.ServiceDidcommMessaging)
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

    public static Service FromDictionary(Dictionary<string, object> dict)
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

        return new Service(
            id: id,
            serviceEndpoint: serviceEndpoint,
            routingKeys: routingKeys,
            accept: accept, type: type);
    }
}