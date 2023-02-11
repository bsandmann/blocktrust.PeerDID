using System.Text.Json;
using System.Xml;
using Blocktrust.PeerDID.Exceptions;

namespace Blocktrust.PeerDID.DIDDoc;

class DIDDocPeerDID
{
    public string did { get; set; }
    public List<VerificationMethodPeerDID> authentication { get; set; }
    public List<VerificationMethodPeerDID> keyAgreement { get; set; }
    public List<Service> service { get; set; }


    public DIDDocPeerDID(string did, List<VerificationMethodPeerDID> authentication, List<VerificationMethodPeerDID> keyAgreement, List<Service> service)
    {
        this.did = did;
        this.authentication = authentication;
        this.keyAgreement = keyAgreement;
        this.service = service;
    }
    
    
    public DIDDocPeerDID(string did, List<VerificationMethodPeerDID> authentication)
    {
        this.did = did;
        this.authentication = authentication;
    }

    /// <summary>
    /// Creates a new instance of DIDDocPeerDID from the given DID Doc JSON.
    /// </summary>
    /// <param name="value">value DID Doc JSON</param>
    /// <returns>DIDDoc PeerDID instance</returns>
    /// <exception cref="MalformedPeerDIDException">MalformedPeerDIDDOcException if the input DID Doc JSON is not a valid peerdid DID Doc</exception>
    public static DIDDocPeerDID fromJson(string value)
    {
        try
        {
            return JsonSerializer.Deserialize<DIDDocPeerDID>(value);
        }
        catch (System.Exception e)
        {
            throw new MalformedPeerDIDException(e.Message);
        }
    }

    public List<string> authenticationKids
    {
        get
        {
            List<string> res = new List<string>();
            foreach (var item in authentication)
            {
                res.Add(item.Id);
            }

            return res;
        }
    }

    public List<string> agreementKids
    {
        get
        {
            List<string> res = new List<string>();
            foreach (var item in keyAgreement)
            {
                res.Add(item.Id);
            }

            return res;
        }
    }

    public Dictionary<string, object> toDict()
    {
        Dictionary<string, object> res = new Dictionary<string, object>()
        {
            { "id", did },
            { "authentication", authentication.Select(item => item.ToDict()) }
        };
        if (keyAgreement.Count > 0)
        {
            res.Add("keyAgreement", keyAgreement.Select(item => item.ToDict()));
        }

        if (service != null)
        {
            List<object> serviceList = new List<object>();
            foreach (var item in service)
            {
                if (item is OtherService)
                {
                    serviceList.Add(((OtherService)item).data);
                }
                else if (item is DIDCommServicePeerDID)
                {
                    serviceList.Add(((DIDCommServicePeerDID)item).ToDict());
                }
            }

            res.Add("service", serviceList);
        }

        return res;
    }

    public string toJson()
    {
        //TODO indenting?
        return JsonSerializer.Serialize(this.toDict()); //, Formatting.Indented);
    }
}