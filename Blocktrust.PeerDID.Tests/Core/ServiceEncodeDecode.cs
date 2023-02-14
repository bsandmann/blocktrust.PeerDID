namespace Blocktrust.PeerDID.Tests.Core;

using DIDDoc;
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
        var expected = new List<PeerDidService>
        {
            PeerDidService.FromDictionary(
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
            "eyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0",
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
        var expected = new List<PeerDidService>
        {
            PeerDidService.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-0" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint" },
                }
            )
        };
        var service = PeerDidHelper.DecodeService(
            "eyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9",
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
        var expected = new List<PeerDidService>
        {
            PeerDidService.FromDictionary(
                new Dictionary<string, object>
                {
                    { "id", $"{Fixture.PEER_DID_NUMALGO_2}#didcommmessaging-0" },
                    { "type", "DIDCommMessaging" },
                    { "serviceEndpoint", "https://example.com/endpoint" },
                    { "routingKeys", new List<string> { "did:example:somemediator#somekey" } },
                    { "accept", new List<string> { "didcomm/v2", "didcomm/aip2;env=rfc587" } },
                }
            ),
            PeerDidService.FromDictionary(
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
            "W3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il0sImEiOlsiZGlkY29tbS92MiIsImRpZGNvbW0vYWlwMjtlbnY9cmZjNTg3Il19LHsidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQyIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTIiXX1d",
            new PeerDid(Fixture.PEER_DID_NUMALGO_2)
        );
        Assert.Equivalent(expected, service);
    }
}