using System.Text.Json;
using Blocktrust.PeerDID.Exceptions;

namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Common.Converter;
using Core;

public class DidDocPeerDid
{
    [JsonPropertyName(DidDocConstants.Id)] public string Did { get; set; }
    [JsonPropertyName(DidDocConstants.Authentication)] public List<VerificationMethodPeerDid> Authentication { get; set; }

    [JsonPropertyName(DidDocConstants.KeyAgreement)] public List<VerificationMethodPeerDid> KeyAgreement { get; set; } = new List<VerificationMethodPeerDid>();
    [JsonPropertyName(DidDocConstants.Service)] public List<PeerDidService>? Service { get; set; }


    public DidDocPeerDid(string did, List<VerificationMethodPeerDid> authentication, List<VerificationMethodPeerDid> keyAgreement, List<PeerDidService> service)
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
            throw new MalformedPeerDIDDocException(e);
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
            { DidDocConstants.Id, Did },
            { DidDocConstants.Authentication, Authentication.Select(item => item.ToDict()) }
        };
        if (KeyAgreement.Count > 0)
        {
            res.Add(DidDocConstants.KeyAgreement, KeyAgreement.Select(item => item.ToDict()));
        }

        if (Service != null)
        {
            List<object> serviceList = new List<object>();
            foreach (var item in Service)
            {
                serviceList.Add(item.ToDict());
            }

            res.Add(DidDocConstants.Service, serviceList);
        }

        return res;
    }

    public string ToJson()
    {
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.WriteIndented = true;
        return JsonSerializer.Serialize(this.ToDict(), serializerOptions);
    }
}