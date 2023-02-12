namespace Blocktrust.PeerDID.DIDDoc;

public class DidCommServicePeerDid : Service
{
    public string Id;
    public string Type;
    public string ServiceEndpoint;
    public List<string> RoutingKeys;
    public List<string> Accept;
    public DidCommServicePeerDid(string id, string type, string serviceEndpoint, List<string> routingKeys, List<string> accept)
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
        res.Add("routingKeys", RoutingKeys);
        res.Add("accept", Accept);
        return res;
    }
}