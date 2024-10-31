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

    // First pass: Process keyAgreement (E) keys first
    foreach (var part in keys.Split('.'))
    {
        if (string.IsNullOrEmpty(part)) continue;
        
        var prefix = part[0];
        var value = part.Substring(1);

        if (prefix == 'E')  // KEY_AGREEMENT
        {
            var decodedAgreemEncumbasis = DecodeMultibaseEncnumbasisAgreement(value, format);
            keyAgreement.Add(new VerificationMethodPeerDid
            {
                Id = $"#key-{keyCounter}", // Using relative ID per spec
                Controller = peerDid.Value,
                VerMaterial = decodedAgreemEncumbasis.VerMaterial
            });
            keyCounter++;
        }
    }

    // Second pass: Process authentication (V) keys
    foreach (var part in keys.Split('.'))
    {
        if (string.IsNullOrEmpty(part)) continue;
        
        var prefix = part[0];
        var value = part.Substring(1);

        if (prefix == 'V')  // AUTHENTICATION
        {
            var decodedAuthEncumbasis = DecodeMultibaseEncnumbasisAuth(value, format);
            authentications.Add(new VerificationMethodPeerDid
            {
                Id = $"#key-{keyCounter}", // Using relative ID per spec
                Controller = peerDid.Value,
                VerMaterial = decodedAuthEncumbasis.VerMaterial
            });
            keyCounter++;
        }
        else if (prefix == 'S')  // SERVICE
        {
            encodedServices.Add(value);
        }
    }

    // Process services
    List<Service>? services = null;
   if (encodedServices.Any())
{
    try
    {
        services = encodedServices.Select((encodedService, index) =>
        {
            // Convert Base64Url to Base64 with validation
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

                // Add padding if needed
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                var decodedBytes = Convert.FromBase64String(base64);
                var decodedJson = System.Text.Encoding.UTF8.GetString(decodedBytes);
                
                // Validate JSON format
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(decodedJson);
                
                // Try to deserialize as array first
                var serviceDataList = new List<Dictionary<string, object>>();
                
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in jsonElement.EnumerateArray())
                    {
                        // Validate service object
                        if (!element.TryGetProperty("t", out _) || !element.TryGetProperty("s", out _))
                        {
                            throw new ArgumentException("Invalid service: missing required fields");
                        }
                        serviceDataList.Add(JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText()));
                    }
                }
                else if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    // Validate service object
                    if (!jsonElement.TryGetProperty("t", out _) || !jsonElement.TryGetProperty("s", out _))
                    {
                        throw new ArgumentException("Invalid service: missing required fields");
                    }
                    serviceDataList.Add(JsonSerializer.Deserialize<Dictionary<string, object>>(decodedJson));
                }
                else
                {
                    throw new ArgumentException("Invalid service: must be object or array");
                }

                // Process each service in the list
                return serviceDataList.Select((serviceData, serviceIndex) =>
                {
                    var serviceEndpoint = new ServiceEndpoint(
                        uri: serviceData["s"].ToString(),
                        routingKeys: serviceData.ContainsKey("r") 
                            ? JsonSerializer.Deserialize<List<string>>(serviceData["r"].ToString())
                            : null,
                        accept: serviceData.ContainsKey("a") 
                            ? JsonSerializer.Deserialize<List<string>>(serviceData["a"].ToString())
                            : null
                    );

                    return new Service(
                        id: serviceIndex == 0 ? "#service" : $"#service-{serviceIndex}",
                        type: serviceData.ContainsKey("t") 
                            ? serviceData["t"].ToString() == "dm" ? "DIDCommMessaging" : serviceData["t"].ToString()
                            : "DIDCommMessaging",
                        serviceEndpoint: serviceEndpoint
                    );
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid service");
            }
        }).SelectMany(x => x).ToList();
    }
    catch (Exception ex)
    {
        throw new ArgumentException("Invalid service");
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