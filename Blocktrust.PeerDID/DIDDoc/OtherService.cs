namespace Blocktrust.PeerDID.DIDDoc;

public class OtherService : Service
{
    public Dictionary<string, object> Data;
    public OtherService(Dictionary<string, object> data)
    {
        this.Data = data;
    }
}