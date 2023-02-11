namespace Blocktrust.PeerDID.DIDDoc;

class OtherService : Service
{
    public Dictionary<string, object> data;
    public OtherService(Dictionary<string, object> data)
    {
        this.data = data;
    }
}