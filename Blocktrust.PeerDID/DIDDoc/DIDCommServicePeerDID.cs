namespace Blocktrust.PeerDID.DIDDoc;

class DIDCommServicePeerDID : Service
{
    public string id;
    public string type;
    public string serviceEndpoint;
    public List<string> routingKeys;
    public List<string> accept;
    public DIDCommServicePeerDID(string id, string type, string serviceEndpoint, List<string> routingKeys, List<string> accept)
    {
        this.id = id;
        this.type = type;
        this.serviceEndpoint = serviceEndpoint;
        this.routingKeys = routingKeys;
        this.accept = accept;
    }

    public Dictionary<string, object> ToDict()
    {
        var res = new Dictionary<string, object>
        {
            { "id", id },
            { "type", type }
        };
        res.Add("serviceEndpoint", serviceEndpoint);
        res.Add("routingKeys", routingKeys);
        res.Add("accept", accept);
        return res;
    }
}