using System.Text.Json;
using Blocktrust.PeerDID.Exceptions;

namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Common.Converter;
using Core;

public class DidDocPeerDid
{
    [JsonPropertyName("id")]
    public string Did { get; set; }
    [JsonPropertyName("authentication")]
    public List<VerificationMethodPeerDid> Authentication { get; set; }

    [JsonPropertyName("keyAgreement")] public List<VerificationMethodPeerDid> KeyAgreement { get; set; } = new List<VerificationMethodPeerDid>();
    [JsonPropertyName("service")]
    public List<Service>? Service { get; set; }

    public DidDocPeerDid()
    {
        
    }
    

    public DidDocPeerDid(string did, List<VerificationMethodPeerDid> authentication, List<VerificationMethodPeerDid> keyAgreement, List<Service> service)
    {
        this.Did = did;
        this.Authentication = authentication;
        this.KeyAgreement = keyAgreement;
        this.Service = service;
    }
    
    
    public DidDocPeerDid(string did, List<VerificationMethodPeerDid> authentication)
    {
        this.Did = did;
        this.Authentication = authentication;
    }

    /// <summary>
    /// Creates a new instance of DIDDocPeerDID from the given DID Doc JSON.
    /// </summary>
    /// <param name="value">value DID Doc JSON</param>
    /// <returns>DIDDoc PeerDID instance</returns>
    /// <exception cref="MalformedPeerDidException">MalformedPeerDIDDOcException if the input DID Doc JSON is not a valid peerdid DID Doc</exception>
    public static DidDocPeerDid FromJson(string value)
    {
        try
        {
            // var serializerOptions = new JsonSerializerOptions();
            // serializerOptions.Converters.Add(new VerificationMethodPeerDIDConverter());
            var ff = JsonNode.Parse(value);
            var obj = ff.AsObject();
            var doc = DidDocHelper.DidDocFromJson(obj);
            return doc;
            // return JsonSerializer.Deserialize<DidDocPeerDid>(value, serializerOptions);
        }
        catch (System.Exception e)
        {
            throw new  MalformedPeerDIDDocException(e);
        }
    }

    public List<string> AuthenticationKids
    {
        get
        {
            List<string> res = new List<string>();
            foreach (var item in Authentication)
            {
                res.Add(item.Id);
            }

            return res;
        }
    }

    public List<string> AgreementKids
    {
        get
        {
            List<string> res = new List<string>();
            foreach (var item in KeyAgreement)
            {
                res.Add(item.Id);
            }

            return res;
        }
    }

    public Dictionary<string, object> ToDict()
    {
        Dictionary<string, object> res = new Dictionary<string, object>()
        {
            { "id", Did },
            { "authentication", Authentication.Select(item => item.ToDict()) }
        };
        if (KeyAgreement.Count > 0)
        {
            res.Add("keyAgreement", KeyAgreement.Select(item => item.ToDict()));
        }

        if (Service != null)
        {
            List<object> serviceList = new List<object>();
            foreach (var item in Service)
            {
                if (item is OtherService)
                {
                    serviceList.Add(((OtherService)item).Data);
                }
                else if (item is DidCommServicePeerDid)
                {
                    serviceList.Add(((DidCommServicePeerDid)item).ToDict());
                }
            }

            res.Add("service", serviceList);
        }

        return res;
    }

    public string ToJson()
    {
        //TODO indenting?
        return JsonSerializer.Serialize(this.ToDict(),SerializationOptions.DisplayIndented );
    }
}