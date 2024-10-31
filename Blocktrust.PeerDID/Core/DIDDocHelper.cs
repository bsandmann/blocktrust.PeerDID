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
        // Validate required fields
        if (!jsonObject.ContainsKey(DidDocConstants.Controller))
            throw new ArgumentException($"No 'controller' field in method {jsonObject}");
        
        // Get and validate verification method type
        var verMaterialType = GetVerMethodType(jsonObject);
        if (verMaterialType == null)
            throw new ArgumentException($"Invalid or missing verification method type in {jsonObject}");
        
        // Determine field and format based on verification type
        var field = verMaterialType is VerificationMethodTypeAgreement ? 
            TypeAgreementVerTypeToField[verMaterialType.Value] :
            TypeAuthenticationVerTypeToField[verMaterialType.Value];
        
        var format = verMaterialType is VerificationMethodTypeAgreement ?
            TypeAgreementVerTypeToFormat[verMaterialType.Value] :
            TypeAuthenticationVerTypeToFormat[verMaterialType.Value];

        // Validate and extract the verification material
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

        // Create verification method with consistent key ID format
        return new VerificationMethodPeerDid
        {
            Id = $"{did}#key-{keyIndex}", // Absolute ID including DID
            Controller = did,
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

    // Initialize with empty collections
    var routingKeys = new List<string>();
    var accept = new List<string>();
    string? uri = null;

    // Extract metadata from the DID string for did:peer:2
    if (did.StartsWith("did:peer:2"))
    {
        var parts = did.Split('.');
        if (parts.Length > 3)
        {
            var encodedMetadata = parts[^1];
            if (encodedMetadata.StartsWith("S"))
            {
                try
                {
                    // Remove the 'S' prefix and decode
                    var base64 = encodedMetadata[1..].Replace('-', '+').Replace('_', '/');
                    while (base64.Length % 4 != 0) base64 += '=';
                    var decodedBytes = Convert.FromBase64String(base64);
                    var decodedJson = System.Text.Encoding.UTF8.GetString(decodedBytes);
                    
                    // Handle both single object and array formats
                    var parsedNode = JsonNode.Parse(decodedJson);
                    JsonObject serviceMetadata;
                    
                    if (parsedNode is JsonArray serviceArray && serviceIndex < serviceArray.Count)
                    {
                        serviceMetadata = serviceArray[serviceIndex].AsObject();
                    }
                    else if (parsedNode is JsonObject singleService)
                    {
                        serviceMetadata = singleService;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid service metadata format");
                    }

                    // Extract service data
                    if (serviceMetadata.ContainsKey("s")) // service endpoint
                    {
                        uri = serviceMetadata["s"].GetValue<string>();
                    }
                    if (serviceMetadata.ContainsKey("r")) // routing keys
                    {
                        routingKeys.AddRange(serviceMetadata["r"].AsArray().Select(x => x.GetValue<string>()));
                    }
                    if (serviceMetadata.ContainsKey("a")) // accept
                    {
                        accept.AddRange(serviceMetadata["a"].AsArray().Select(x => x.GetValue<string>()));
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                        throw;
                    throw new ArgumentException($"Failed to decode service metadata from DID: {ex.Message}");
                }
            }
        }
    }

    // Process serviceEndpoint from the DID Document
    var endpointNode = jsonObject[ServiceConstants.ServiceEndpoint];
    if (endpointNode is JsonValue) // Legacy format with direct URI
    {
        uri = endpointNode.GetValue<string>();
    }
    else if (endpointNode?.AsObject() is JsonObject serviceEndpointObj)
    {
        // New format
        uri = serviceEndpointObj["uri"]?.GetValue<string>();
        
        if (serviceEndpointObj.ContainsKey("routingKeys") && serviceEndpointObj["routingKeys"]?.AsArray() is JsonArray rKeys)
        {
            routingKeys.AddRange(rKeys.Select(x => x.GetValue<string>()));
        }
        
        if (serviceEndpointObj.ContainsKey("accept") && serviceEndpointObj["accept"]?.AsArray() is JsonArray acc)
        {
            accept.AddRange(acc.Select(x => x.GetValue<string>()));
        }
    }

    if (string.IsNullOrEmpty(uri))
    {
        throw new ArgumentException($"No valid URI found in service endpoint {jsonObject}");
    }
    var serviceEndpoint = new ServiceEndpoint(
        uri: uri,
        routingKeys: routingKeys.Distinct().ToList(),
        accept: accept.Distinct().ToList()
    );

    // Always use relative references for service IDs as per spec
    string serviceId;
    if (jsonObject.ContainsKey("id"))
    {
        // If ID is provided, use it as-is if relative, or extract the fragment if absolute
        var rawId = jsonObject["id"].GetValue<string>();
        serviceId = rawId.StartsWith("#") ? rawId : "#" + rawId.Split("#").Last();
    }
    else
    {
        // Generate relative ID if none provided
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