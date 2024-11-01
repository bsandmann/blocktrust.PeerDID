using System.Text.Json;
using Blocktrust.Common.Models.DidDoc;

namespace Blocktrust.PeerDID.PeerDIDCreateResolve;

using Exceptions;
using Core;
using DIDDoc;
using FluentResults;
using Types;

public static class PeerDidResolver
{
    public static Result<string> ResolvePeerDid(PeerDid peerDid, VerificationMaterialFormatPeerDid format = VerificationMaterialFormatPeerDid.Multibase)
    {
        if (!PeerDidCreator.IsPeerDid(peerDid.Value))
        {
            return Result.Fail($"Does not match peer DID regexp: {peerDid}");
        }

        DidDocPeerDid didDoc;
        try 
        {
            didDoc = peerDid.Value[9] switch 
            {
                '0' => BuildDidDocNumalgo0(peerDid, format),
                '2' => BuildDidDocNumalgo2(peerDid, format),
                _ => throw new ArgumentException($"Invalid numalgo of Peer DID: {peerDid}")
            };
        }
        catch (Exception e)
        {
            return Result.Fail($"Error resolving Peer DID: {e.Message}");
        }

        return Result.Ok(didDoc.ToJson());
    }

    private static DidDocPeerDid BuildDidDocNumalgo0(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        var inceptionKey = peerDid.Value.Substring(10);
        var decodedEncumbasis = DecodeMultibaseEncnumbasisAuth(inceptionKey, format);
        
        return new DidDocPeerDid(
            did: peerDid.Value,
            authentications: new List<VerificationMethodPeerDid>
            {
                PeerDidHelper.GetVerificationMethod(peerDid.Value, decodedEncumbasis)
            });
    }
    
    private static DidDocPeerDid BuildDidDocNumalgo2(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
{
    var keys = peerDid.Value.Substring(11);
    var encodedServices = new List<string>();
    var authentications = new List<VerificationMethodPeerDid>();
    var keyAgreement = new List<VerificationMethodPeerDid>();
    var keyCounter = 1;

    // First process encryption keys to maintain key numbering
    foreach (var part in keys.Split('.'))
    {
        if (string.IsNullOrEmpty(part)) continue;
        var prefix = part[0];
        var value = part.Substring(1);
        if (prefix == 'E')  
        {
            var decodedAgreemEncumbasis = DecodeMultibaseEncnumbasisAgreement(value, format);
            keyAgreement.Add(new VerificationMethodPeerDid
            {
                Id = $"#key-{keyCounter}", 
                Controller = peerDid.Value,
                VerMaterial = decodedAgreemEncumbasis.VerMaterial
            });
            keyCounter++;
        }
    }

    // Then process authentication keys and services
    foreach (var part in keys.Split('.'))
    {
        if (string.IsNullOrEmpty(part)) continue;
        var prefix = part[0];
        var value = part.Substring(1);
        if (prefix == 'V')  
        {
            var decodedAuthEncumbasis = DecodeMultibaseEncnumbasisAuth(value, format);
            authentications.Add(new VerificationMethodPeerDid
            {
                Id = $"#key-{keyCounter}", 
                Controller = peerDid.Value,
                VerMaterial = decodedAuthEncumbasis.VerMaterial
            });
            keyCounter++;
        }
        else if (prefix == 'S')  
        {
            encodedServices.Add(value);
        }
    }
List<Service>? services = null;
if (encodedServices.Any())
{
    try 
    {
        services = new List<Service>();
        var serviceIndex = 0; // Track service index across all encoded services

        foreach(var encodedService in encodedServices)
        {
            if (string.IsNullOrEmpty(encodedService))
            {
                throw new ArgumentException("Invalid service: empty service data");
            }

            string base64;
            try 
            {
                base64 = encodedService
                    .Replace('-', '+')
                    .Replace('_', '/');
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }
                var decodedBytes = Convert.FromBase64String(base64);
                var decodedJson = System.Text.Encoding.UTF8.GetString(decodedBytes);
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(decodedJson);

                List<JsonElement> serviceElements;
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    serviceElements = jsonElement.EnumerateArray().ToList();
                }
                else
                {
                    serviceElements = new List<JsonElement> { jsonElement };
                }

                foreach(var element in serviceElements)
                {
                    if (!element.TryGetProperty("t", out _) || !element.TryGetProperty("s", out _))
                    {
                        throw new ArgumentException("Invalid service: missing required fields");
                    }

                    var serviceType = element.GetProperty("t").GetString() == "dm"
                        ? "DIDCommMessaging"
                        : element.GetProperty("t").GetString();

                    var serviceEndpointElement = element.GetProperty("s");
                    string uri;
                    List<string?> routingKeys = null;
                    List<string?> accept = null;

                    if (serviceEndpointElement.ValueKind == JsonValueKind.String)
                    {
                        uri = serviceEndpointElement.GetString();
                        
                        if (element.TryGetProperty("r", out var routingKeysElement))
                        {
                            routingKeys = routingKeysElement.EnumerateArray()
                                .Select(x => x.GetString())
                                .Where(x => x != null)
                                .ToList();
                        }
                        
                        if (element.TryGetProperty("a", out var acceptElement))
                        {
                            accept = acceptElement.EnumerateArray()
                                .Select(x => x.GetString())
                                .Where(x => x != null)
                                .ToList();
                        }
                    }
                    else
                    {
                        // Handle case where "s" is an object
                        uri = serviceEndpointElement.GetProperty("uri").GetString();
                        
                        if (serviceEndpointElement.TryGetProperty("r", out var routingKeysElement))
                        {
                            routingKeys = routingKeysElement.EnumerateArray()
                                .Select(x => x.GetString())
                                .Where(x => x != null)
                                .ToList();
                        }
                        
                        if (serviceEndpointElement.TryGetProperty("a", out var acceptElement))
                        {
                            accept = acceptElement.EnumerateArray()
                                .Select(x => x.GetString())
                                .Where(x => x != null)
                                .ToList();
                        }
                    }

                    var serviceEndpoint = new ServiceEndpoint(
                        uri: uri,
                        routingKeys: routingKeys,
                        accept: accept
                    );

                    services.Add(new Service(
                        id: serviceIndex == 0 ? "#service" : $"#service-{serviceIndex}",
                        type: serviceType,
                        serviceEndpoint: serviceEndpoint
                    ));
                    serviceIndex++;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid service: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        throw new ArgumentException($"Invalid service: {ex.Message}");
    }
}
    return new DidDocPeerDid(
        did: peerDid.Value,
        authentications: authentications,
        keyAgreements: keyAgreement,
        services: services
    );
}
    
    private static DecodedEncumbasis DecodeMultibaseEncnumbasisAuth(
        string multibase,
        VerificationMaterialFormatPeerDid format
    )
    {
        try
        {
            DecodedEncumbasis decodedEncumbasis = PeerDidHelper.DecodeMultibaseEncnumbasis(multibase, format);
            Validation.ValidateAuthenticationMaterialType(decodedEncumbasis.VerMaterial);
            return decodedEncumbasis;
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDidException($"Invalid key {multibase}", e);
        }
    }

    private static DecodedEncumbasis DecodeMultibaseEncnumbasisAgreement(
        string multibase,
        VerificationMaterialFormatPeerDid format
    )
    {
        try
        {
            DecodedEncumbasis decodedEncumbasis = PeerDidHelper.DecodeMultibaseEncnumbasis(multibase, format);
            Validation.ValidateAgreementMaterialType(decodedEncumbasis.VerMaterial);
            return decodedEncumbasis;
        }
        catch (ArgumentException e)
        {
            throw new MalformedPeerDidException($"Invalid key {multibase}", e);
        }
    }
}