namespace Blocktrust.PeerDID.Tests.Core;

using Common.Models.DidDoc;
using PeerDID.Core;
using Types;

public class ServiceEncodeDecode
{
    [Fact]
    public void TestEncodeService()
    {
        Assert.Equal(
            ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0",
            PeerDidHelper.EncodeService(
                """
                        {
                            "type": "DIDCommMessaging",
                            "serviceEndpoint": "https://example.com/endpoint",
                            "routingKeys": ["did:example:somemediator#somekey"],
                            "accept": ["didcomm/v2", "didcomm/aip2;env=rfc587"]
                        }
                        """
            )
        );
    }

    [Fact]
    public void TestDecodeService()
    {
        var expected = new List<Service>
        {
            Service.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-0" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint" },
                    { "routingKeys", new List<string> { "did:example:somemediator#somekey" } },
                    { "accept", new List<string> { "didcomm/v2", "didcomm/aip2;env=rfc587" } },
                })
        };
        var service = PeerDidHelper.DecodeService(
            new List<string> 
            { 
                "eyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0" 
            },
            new PeerDid(Fixture.PEER_DID_NUMALGO_2)
        );
        Assert.Equivalent(expected, service);
    }
    [Fact]
    public void TestEncodeServiceMinimalFields()
    {
        Assert.Equal(
            ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9",
            PeerDidHelper.EncodeService(
                """
                        {
                            "type": "DIDCommMessaging",
                            "serviceEndpoint": "https://example.com/endpoint"
                        }
                        """
            )
        );
    }

    [Fact]
    public void TestDecodeServiceMinimalFields()
    {
        var expected = new List<Service>
        {
            Service.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-0" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint" },
                }
            )
        };
        var service = PeerDidHelper.DecodeService(
            new List<string> 
            { 
                "eyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9" 
            },
            new PeerDid(Fixture.PEER_DID_NUMALGO_2)
        );
        Assert.Equivalent(expected, service);
    }


    [Fact]
    public void TestEncodeServiceMultipleEntries()
    {
        Assert.Equivalent(
            ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il0sImEiOlsiZGlkY29tbS92MiIsImRpZGNvbW0vYWlwMjtlbnY9cmZjNTg3Il19LHsidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQyIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTIiXX1d",
            PeerDidHelper.EncodeService(
                """
                        [
                            {
                                "type": "DIDCommMessaging",
                                "serviceEndpoint": "https://example.com/endpoint",
                                "routingKeys": ["did:example:somemediator#somekey"],
                                "accept": ["didcomm/v2", "didcomm/aip2;env=rfc587"]
                            },
                            {
                                "type": "DIDCommMessaging",
                                "serviceEndpoint": "https://example.com/endpoint2",
                                "routingKeys": ["did:example:somemediator#somekey2"]
                            }
                        ]
                        """
            )
        );
    }
    [Fact]
    public void TestDecodeServiceMultipleEntries()
    {
        var expected = new List<Service>
        {
            Service.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-0" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint" },
                    { "routingKeys", new List<string> { "did:example:somemediator#somekey" } },
                    { "accept", new List<string> { "didcomm/v2", "didcomm/aip2;env=rfc587" } },
                }
            ),
            Service.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-1" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint2" },
                    { "routingKeys", new List<string> { "did:example:somemediator#somekey2" } },
                }
            )
        };
        var service = PeerDidHelper.DecodeService(
            new List<string> 
            { 
                "W3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il0sImEiOlsiZGlkY29tbS92MiIsImRpZGNvbW0vYWlwMjtlbnY9cmZjNTg3Il19LHsidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQyIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTIiXX1d" 
            },
            new PeerDid(Fixture.PEER_DID_NUMALGO_2)
        );
        Assert.Equivalent(expected, service);
    }

    [Fact]
    public void TestDecodeServiceMultipleIndividuallyEncodedEntries()
    {
        var expected = new List<Service>
        {
            Service.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-0" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint" },
                    { "routingKeys", new List<string> { "did:example:somemediator#somekey" } },
                }
            ),
            Service.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-1" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint2" },
                    { "routingKeys", new List<string> { "did:example:somemediator#somekey2" } },
                }
            )
        };
    
        var services = new List<string>
        {
            "eyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXX0K",
            "eyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdfQ"
        };
    
        // Pass the services list directly to DecodeService
        var service = PeerDidHelper.DecodeService(
            services,
            new PeerDid(Fixture.PEER_DID_NUMALGO_2)
        );
    
        Assert.Equivalent(expected, service);
    }
    
    // [Fact]
    // public void TestResolveFullPeerDidWithMultipleServices()
    // {
    //     // A complete peer DID with individually encoded services
    //     var peerDid = "did:peer:2" + 
    //                   ".Vz6Mkj3PUd1WjvaDhNZhhhXQdz5UnZXmS7ehtx8bsPpD47kKc" +
    //                   ".Ez6LSg8zQom395jKLrGiBNruB9MM6V8PWuf2FpEy4uRFiqQBR" +
    //                   ".SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHA6Ly9leGFtcGxlLmNvbS9kaWRjb21tIiwiYSI6WyJkaWRjb21tL3YyIl0sInIiOlsiZGlkOmV4YW1wbGU6MTIzNDU2Nzg5YWJjZGVmZ2hpI2tleS0xIl19fQ" +
    //                   ".SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHA6Ly9leGFtcGxlLmNvbS9hbm90aGVyIiwiYSI6WyJkaWRjb21tL3YyIl0sInIiOlsiZGlkOmV4YW1wbGU6MTIzNDU2Nzg5YWJjZGVmZ2hpI2tleS0yIl19fQ";
    //
    //     var result = PeerDidResolver.ResolvePeerDid(new PeerDid(peerDid), VerificationMaterialFormatPeerDid.Multibase);
    //
    //     Assert.True(result.IsSuccess);
    //     var didDoc = DidDocPeerDid.FromJson(result.Value);
    //     Assert.NotNull(didDoc?.Value?.Services);
    //     Assert.Equal(2, didDoc.Value.Services.Count);
    //
    //     // Verify first service
    //     var firstService = didDoc.Value.Services[0];
    //     Assert.Equal("DIDCommMessaging", firstService.Type);
    //     Assert.Equal("http://example.com/didcomm", firstService.ServiceEndpoint);
    //     Assert.Contains("did:example:123456789abcdefghi#key-1", firstService.RoutingKeys);
    //
    //     // Verify second service
    //     var secondService = didDoc.Value.Services[1];
    //     Assert.Equal("DIDCommMessaging", secondService.Type);
    //     Assert.Equal("http://example.com/another", secondService.ServiceEndpoint);
    //     Assert.Contains("did:example:123456789abcdefghi#key-2", secondService.RoutingKeys);
    // }
}