using System.Text.Json;

namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Common.Models.DidDoc;
using Core;
using FluentResults;

public class DidDocPeerDid
{
    [JsonPropertyName(DidDocConstants.Id)] 
    public string Did { get; set; }
    
    [JsonPropertyName(DidDocConstants.Authentication)] 
    public List<VerificationMethodPeerDid> Authentications { get; set; }

    [JsonPropertyName(DidDocConstants.KeyAgreement)] 
    public List<VerificationMethodPeerDid> KeyAgreements { get; set; } = new List<VerificationMethodPeerDid>();
    
    [JsonPropertyName(DidDocConstants.Service)] 
    public List<Service>? Services { get; set; }

    // Updated context per newer spec
    private static readonly string[] DefaultContext = new[]
    {
        "https://www.w3.org/ns/did/v1",
        "https://w3id.org/security/suites/ed25519-2020/v1",
        "https://w3id.org/security/suites/x25519-2020/v1"
    };

    public DidDocPeerDid(string did, List<VerificationMethodPeerDid> authentications, List<VerificationMethodPeerDid> keyAgreements, List<Service>? services)
    {
        Did = did;
        Authentications = authentications;
        KeyAgreements = keyAgreements;
        Services = services;
    }

    public DidDocPeerDid(string did, List<VerificationMethodPeerDid> authentications)
    {
        Did = did;
        Authentications = authentications;
        KeyAgreements = new List<VerificationMethodPeerDid>();
        Services = null;
    }

    public static Result<DidDocPeerDid> FromJson(string value)
    {
        try
        {
            var jsonNode = JsonNode.Parse(value);
            if (jsonNode == null || jsonNode is not JsonObject jsonObject)
            {
                return Result.Fail("Invalid JSON input: Root element must be an object.");
            }

            // Ensure 'id' is present and of expected type
            if (!jsonObject.ContainsKey(DidDocConstants.Id) || jsonObject[DidDocConstants.Id] is not JsonValue)
            {
                return Result.Fail("Missing or invalid 'id' field: Expected a JSON string.");
            }

            var doc = DidDocHelper.DidDocFromJson(jsonObject);
            return Result.Ok(doc);
        }
        catch (InvalidOperationException e) when (e.Message.Contains("JsonObject"))
        {
            return Result.Fail("DIDDoc could not be parsed from JSON: Expected JSON object but received another type.");
        }
        catch (Exception e)
        {
            return Result.Fail($"DIDDoc could not be parsed from JSON: {e.Message}");
        }
    }

    public List<string> AuthenticationKids => Authentications.Select(item => item.Id).ToList();

    public List<string> AgreementKids => KeyAgreements.Select(item => $"{Did}#{item.Id}").ToList();

    public Dictionary<string, object> ToDict()
    {
        var res = new Dictionary<string, object>
        {
            { "@context", DefaultContext },
            { DidDocConstants.Id, Did },
            { DidDocConstants.Authentication, Authentications.Select(item => item.ToDict()).ToList() }
        };

        if (KeyAgreements.Any())
        {
            res.Add(DidDocConstants.KeyAgreement, KeyAgreements.Select(item => item.ToDict()).ToList());
        }
        
        if (Services?.Any() == true)
        {
            res.Add(DidDocConstants.Service, Services.Select(item => item.ToDict()).ToList());
        }
        
        return res;
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(ToDict(), options);
    }
}