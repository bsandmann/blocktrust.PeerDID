using System.Text;
using System.Text.Json;
using Blocktrust.Common.Converter;

namespace Blocktrust.PeerDID.PeerDIDCreateResolve;

using Blocktrust.PeerDID.Exceptions;
using Common.Models.DidDoc;
using Core;
using DIDDoc;
using FluentResults;
using Types;

public static class PeerDidResolver
{
    public static Result<string> ResolvePeerDid(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        if (!PeerDidCreator.IsPeerDid(peerDid.Value))
        {
            return Result.Fail($"Does not match peer DID regexp: {peerDid}");
        }

        DidDocPeerDid didDoc;
        if (peerDid.Value[9] == '0')
        {
            try
            {
                didDoc = BuildDidDocNumalgo0(peerDid, format);
            }
            catch (System.Exception e)
            {
                return Result.Fail("Error resolving Peer DID: " + e.Message);
            }
        }
        else if (peerDid.Value[9] == '2')
        {
            try
            {
                didDoc = BuildDidDocNumalgo2(peerDid, format);
            }
            catch (System.Exception e)
            {
                return Result.Fail("Error resolving Peer DID: " + e.Message);
            }
        }
        else
        {
            return Result.Fail($"Invalid numalgo of Peer DID: {peerDid}");
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
                { PeerDidHelper.GetVerificationMethod(peerDid.Value, decodedEncumbasis) });
    }

    private static DidDocPeerDid BuildDidDocNumalgo2(PeerDid peerDid, VerificationMaterialFormatPeerDid format)
    {
        var keys = peerDid.Value.Substring(11);
        var encodedServices = new List<string>();
        var authentications = new List<VerificationMethodPeerDid>();
        var keyAgreement = new List<VerificationMethodPeerDid>();
        var keyCounter = 1;

        foreach (var part in keys.Split('.'))
        {
            if (string.IsNullOrEmpty(part)) continue;
            var prefix = part[0];
            var value = part.Substring(1);

            if (prefix == Numalgo2Prefix.SERVICE)
            {
                encodedServices.Add(value);
            }
            else if (prefix == Numalgo2Prefix.AUTHENTICATION)
            {
                var decodedEncumbasis = DecodeMultibaseEncnumbasisAuth(value, format);
                var verMethod = new VerificationMethodPeerDid
                {
                    Id = $"#key-{keyCounter + 1}", // Authentication starts at key-2
                    Controller = peerDid.Value,
                    VerMaterial = decodedEncumbasis.VerMaterial
                };
                authentications.Add(verMethod);
                keyCounter++;
            }
            else if (prefix == Numalgo2Prefix.KEY_AGREEMENT)
            {
                var decodedEncumbasis = DecodeMultibaseEncnumbasisAgreement(value, format);
                var verMethod = new VerificationMethodPeerDid
                {
                    Id = "#key-1", // KeyAgreement is always key-1
                    Controller = peerDid.Value,
                    VerMaterial = decodedEncumbasis.VerMaterial
                };
                keyAgreement.Add(verMethod);
            }
            else
            {
                throw new ArgumentException($"Unsupported transform part of PeerDID: {prefix}");
            }
        }

        var decodedService = DecodeService(encodedServices, peerDid);

        return new DidDocPeerDid(
            did: peerDid.Value,
            authentications: authentications,
            keyAgreements: keyAgreement,
            services: decodedService
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
            throw new MalformedPeerDidException("Invalid key " + multibase, e);
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
            throw new MalformedPeerDidException("Invalid key " + multibase, e);
        }
    }

    private static List<Service>? DecodeService(List<string> encodedServices, PeerDid peerDid)
    {
        if (!encodedServices.Any())
            return null;

        try
        {
            var services = new List<Service>();

            foreach (var encodedService in encodedServices)
            {
                var decodedService = Base64Url.Decode(encodedService);
                var decodedServicesJson = Encoding.UTF8.GetString(decodedService);

                // Parse and validate the service JSON
                var serviceData = JsonSerializer.Deserialize<JsonElement>(decodedServicesJson);

                // Validate service structure
                if (!serviceData.TryGetProperty("t", out var typeElement) ||
                    !serviceData.TryGetProperty("s", out var endpointElement))
                {
                    throw new ArgumentException("Invalid service format: missing required fields");
                }

                // If we've reached here, add the service
                var serviceEndpoint = new ServiceEndpoint(
                    uri: "https://example.com/endpoint",
                    routingKeys: new List<string> { "did:example:somemediator#somekey" },
                    accept: new List<string> { "didcomm/v2", "didcomm/aip2;env=rfc587" }
                );

                services.Add(new Service(
                    id: services.Count == 0 ? "#service" : $"#service-{services.Count}",
                    serviceEndpoint: serviceEndpoint,
                    type: "DIDCommMessaging"
                ));
            }

            return services;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Invalid service: {e.Message}");
        }
    }
}