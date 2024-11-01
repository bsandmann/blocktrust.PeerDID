using System.Text.Json;
using System.Text.Json.Nodes;
using Blocktrust.Common.Models.DidDoc;
using Blocktrust.PeerDID.DIDDoc;
using Blocktrust.PeerDID.Types;

namespace Blocktrust.PeerDID.Core;

public static class DidDocHelper 
{
    private static readonly Dictionary<string, string> TypeAgreementVerTypeToField = new()
    {
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2019.Value, PublicKeyFieldValues.Base58 },
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2020.Value, PublicKeyFieldValues.Multibase },
        { VerificationMethodTypeAgreement.JsonWebKey2020.Value, PublicKeyFieldValues.Jwk },
    };

    private static readonly Dictionary<string, string> TypeAuthenticationVerTypeToField = new()
    {
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2018.Value, PublicKeyFieldValues.Base58 },
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2020.Value, PublicKeyFieldValues.Multibase },
        { VerificationMethodTypeAuthentication.JsonWebKey2020.Value, PublicKeyFieldValues.Jwk },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> TypeAgreementVerTypeToFormat = new()
    {
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2019.Value, VerificationMaterialFormatPeerDid.Base58 },
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2020.Value, VerificationMaterialFormatPeerDid.Multibase },
        { VerificationMethodTypeAgreement.JsonWebKey2020.Value, VerificationMaterialFormatPeerDid.Jwk },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> TypeAuthenticationVerTypeToFormat = new()
    {
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2018.Value, VerificationMaterialFormatPeerDid.Base58 },
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2020.Value, VerificationMaterialFormatPeerDid.Multibase },
        { VerificationMethodTypeAuthentication.JsonWebKey2020.Value, VerificationMaterialFormatPeerDid.Jwk },
    };

    public static DidDocPeerDid DidDocFromJson(JsonObject jsonObject)
    {
        // Validate and extract DID
        jsonObject.TryGetPropertyValue(DidDocConstants.Id, out JsonNode? didJsonNode);
        var did = didJsonNode?.AsValue().ToString() ?? 
            throw new ArgumentException("No 'id' field");

        var keyIndex = 1; // Initialize key index counter
        var verificationMethods = new List<VerificationMethodPeerDid>();
        
        // Process keyAgreement first to maintain consistent key ordering
        jsonObject.TryGetPropertyValue(DidDocConstants.KeyAgreement, out JsonNode? keyAgreementJsonNode);
        var keyAgreement = new List<VerificationMethodPeerDid>();
        if (keyAgreementJsonNode is not null)
        {
            keyAgreement = keyAgreementJsonNode.AsArray()
                .Select(p => ProcessVerificationMethod(p.AsObject(), did, keyIndex++))
                .ToList();
            verificationMethods.AddRange(keyAgreement);
        }

        // Process authentication methods next
        var authentication = new List<VerificationMethodPeerDid>();
        jsonObject.TryGetPropertyValue(DidDocConstants.Authentication, out JsonNode? authenticationJsonNode);
        if (authenticationJsonNode is not null)
        {
            authentication = authenticationJsonNode.AsArray()
                .Select(p => ProcessVerificationMethod(p.AsObject(), did, keyIndex++))
                .ToList();
            verificationMethods.AddRange(authentication);
        }

        List<Service>? services = null;
        jsonObject.TryGetPropertyValue(DidDocConstants.Service, out JsonNode? serviceJsonNode);
        if (serviceJsonNode is not null)
        {
            if (serviceJsonNode is JsonArray serviceArray)
            {
                services = serviceArray
                    .Select((p, index) => ProcessService(p.AsObject(), did, index))
                    .ToList();
            }
            else if (serviceJsonNode is JsonObject singleService)
            {
                services = new List<Service> { ProcessService(singleService, did, 0) };
            }
        }
        return new DidDocPeerDid(did, authentication, keyAgreement, services);
    }
private static VerificationMethodPeerDid ProcessVerificationMethod(JsonObject jsonObject, string did, int keyIndex)
{
    // Verify all required fields exist before processing
    if (!jsonObject.ContainsKey(DidDocConstants.Controller))
        throw new ArgumentException($"No 'controller' field in method {jsonObject}");
    
    var controller = jsonObject[DidDocConstants.Controller]?.ToString() ??
        throw new ArgumentException($"No 'controller' field in method {jsonObject}");

    // Get verification type and validate it exists
    var verMaterialType = GetVerMethodType(jsonObject);
    if (verMaterialType == null)
        throw new ArgumentException($"Invalid or missing verification method type in {jsonObject}");
    
    var field = verMaterialType is VerificationMethodTypeAgreement ? 
        TypeAgreementVerTypeToField[verMaterialType.Value] :
        TypeAuthenticationVerTypeToField[verMaterialType.Value];
    
    var format = verMaterialType is VerificationMethodTypeAgreement ?
        TypeAgreementVerTypeToFormat[verMaterialType.Value] :
        TypeAuthenticationVerTypeToFormat[verMaterialType.Value];

    if (!jsonObject.ContainsKey(field))
        throw new ArgumentException($"No '{field}' field in method {jsonObject}");

    object value;
    if (verMaterialType == VerificationMethodTypeAgreement.JsonWebKey2020 ||
        verMaterialType == VerificationMethodTypeAuthentication.JsonWebKey2020)
    {
        var jwkJson = JsonSerializer.Deserialize<PeerDidJwk>(jsonObject[field]);
        if (jwkJson == null) 
            throw new ArgumentException($"Invalid JWK in method {jsonObject}");
        value = jwkJson;
    }
    else
    {
        if (jsonObject[field] == null)
            throw new ArgumentException($"Missing value for field '{field}' in method {jsonObject}");
            
        value = jsonObject[field].GetValue<string>();
    }

    // Use spec-compliant #key-N format for all DID methods
    return new VerificationMethodPeerDid
    {
        Id = $"{did}#key-{keyIndex}",
        Controller = controller,
        VerMaterial = new VerificationMaterialPeerDid<VerificationMethodTypePeerDid>(
            format: format,
            type: verMaterialType,
            value: value)
    };
}

private static Service ProcessService(JsonObject jsonObject, string did, int serviceIndex)
{
    if (jsonObject == null || !jsonObject.ContainsKey(ServiceConstants.ServiceType))
    {
        throw new ArgumentException($"Invalid or missing fields in service JSON: {jsonObject}");
    }

    var type = jsonObject[ServiceConstants.ServiceType]?.ToString() ??
               throw new ArgumentException($"No 'type' field in service {jsonObject}");

    var serviceEndpointNode = jsonObject[ServiceConstants.ServiceEndpoint];
    if (serviceEndpointNode == null)
    {
        throw new ArgumentException($"No service endpoint in service {jsonObject}");
    }

    string? uri = null;
    List<string> routingKeys = new();
    List<string> accept = new();

    if (serviceEndpointNode is JsonValue)
    {
        uri = serviceEndpointNode.GetValue<string>();
    }
    else if (serviceEndpointNode is JsonObject serviceEndpointObj)
    {
        uri = serviceEndpointObj["uri"]?.GetValue<string>();

        if (serviceEndpointObj.ContainsKey("routingKeys"))
        {
            var routingKeysArray = serviceEndpointObj["routingKeys"]?.AsArray();
            if (routingKeysArray != null)
            {
                routingKeys.AddRange(routingKeysArray.Select(x => x?.GetValue<string>()).Where(x => x != null));
            }
        }

        if (serviceEndpointObj.ContainsKey("accept"))
        {
            var acceptArray = serviceEndpointObj["accept"]?.AsArray();
            if (acceptArray != null)
            {
                accept.AddRange(acceptArray.Select(x => x?.GetValue<string>()).Where(x => x != null));
            }
        }
    }

    if (string.IsNullOrEmpty(uri))
    {
        throw new ArgumentException($"No valid URI found in service endpoint {jsonObject}");
    }

    var serviceEndpoint = new ServiceEndpoint(
        uri: uri,
        routingKeys: routingKeys.Any() ? routingKeys : null,
        accept: accept.Any() ? accept : null
    );

    string serviceId;
    if (jsonObject.ContainsKey("id"))
    {
        var rawId = jsonObject["id"].GetValue<string>();
        serviceId = rawId.StartsWith("#") ? rawId : "#" + rawId.Split("#").Last();
    }
    else
    {
        serviceId = serviceIndex == 0 ? "#service" : $"#service-{serviceIndex}";
    }

    return new Service(
        id: serviceId,
        serviceEndpoint: serviceEndpoint,
        type: type);
}
private static VerificationMethodTypePeerDid GetVerMethodType(JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue(DidDocConstants.Type, out JsonNode? typeJsonNode);
        var type = typeJsonNode?.AsValue().ToString() ?? 
            throw new ArgumentException($"No 'type' field in method {jsonObject}");

        if (type == VerificationMethodTypeAgreement.X25519KeyAgreementKey2019.Value)
            return VerificationMethodTypeAgreement.X25519KeyAgreementKey2019;
        
        if (type == VerificationMethodTypeAgreement.X25519KeyAgreementKey2020.Value)
            return VerificationMethodTypeAgreement.X25519KeyAgreementKey2020;
        
        if (type == VerificationMethodTypeAuthentication.Ed25519VerificationKey2018.Value)
            return VerificationMethodTypeAuthentication.Ed25519VerificationKey2018;
        
        if (type == VerificationMethodTypeAuthentication.Ed25519VerificationKey2020.Value)
            return VerificationMethodTypeAuthentication.Ed25519VerificationKey2020;
        
        if (type == VerificationMethodTypeAuthentication.JsonWebKey2020.Value)
        {
            var jwkNode = jsonObject[PublicKeyFieldValues.Jwk] ??
                          throw new ArgumentException($"No 'jwk' field in method {jsonObject}");
            var crv = jwkNode["crv"]?.GetValue<string>() ??
                      throw new ArgumentException($"No 'crv' field in method {jsonObject}");
            
            return crv == "X25519" 
                ? VerificationMethodTypeAgreement.JsonWebKey2020 
                : VerificationMethodTypeAuthentication.JsonWebKey2020;
        }
    
        throw new ArgumentException($"Unknown verification method type {type}");
    }
}