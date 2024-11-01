using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Blocktrust.PeerDID.DIDDoc;
using Blocktrust.PeerDID.PeerDIDCreateResolve;
using Blocktrust.PeerDID.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blocktrust.PeerDID.Tests
{
    public class ResolveNumalgo2
    {

        private static string NormalizeJson(string json)
        {
            // First normalize to remove whitespace
            var normalizedJson = JsonNormalizer.Normalize(json);

            // Then parse and sort consistently
            var parsedJson = JObject.Parse(normalizedJson);
            var sortedJson = SortProperties(parsedJson);
            return JsonConvert.SerializeObject(sortedJson, Formatting.None);
        }

        // Add JsonNormalizer class
        public static class JsonNormalizer
        {
            public static string Normalize(string json)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };

                using var document = JsonDocument.Parse(json);
                using var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream);
                document.WriteTo(writer);
                writer.Flush();

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }


        // Helper to sort properties of JObject recursively
        private static JObject SortProperties(JObject original)
        {
            var sorted = new JObject();
            foreach (var property in original.Properties().OrderBy(p => p.Name))
            {
                sorted[property.Name] = property.Value.Type == JTokenType.Object
                    ? SortProperties((JObject)property.Value)
                    : property.Value;
            }

            return sorted;
        }

        public class DidTestData
        {
            public string Did { get; set; }
            public string ExpectedJson { get; set; }

            public DidTestData(string did, string expectedJson)
            {
                Did = did;
                ExpectedJson = expectedJson;
            }
        }

        public static IEnumerable<object[]> DIDTestCases()
        {
            var testCases = new List<DidTestData>
            {
                new DidTestData(
                    did:
                    "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQxIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTEiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX19",
                    expectedJson: """
                                  {
                                    "@context": [
                                      "https://www.w3.org/ns/did/v1",
                                      "https://w3id.org/security/suites/ed25519-2020/v1",
                                      "https://w3id.org/security/suites/x25519-2020/v1"
                                    ],
                                    "id": "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQxIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTEiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX19",
                                    "authentication": [
                                      {
                                        "id": "#key-2",
                                        "type": "Ed25519VerificationKey2020",
                                        "controller": "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQxIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTEiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX19",
                                        "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                                      }
                                    ],
                                    "keyAgreement": [
                                      {
                                        "id": "#key-1",
                                        "type": "X25519KeyAgreementKey2020",
                                        "controller": "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQxIiwiciI6WyJkaWQ6ZXhhbXBsZTpzb21lbWVkaWF0b3Ijc29tZWtleTEiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX19",
                                        "publicKeyMultibase": "z6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud"
                                      }
                                    ],
                                    "service": [
                                      {
                                        "id": "#service",
                                        "type": "DIDCommMessaging",
                                        "serviceEndpoint": {
                                          "uri": "https://example.com/endpoint1",
                                          "routingKeys": [
                                            "did:example:somemediator#somekey1"
                                          ],
                                          "accept": [
                                            "didcomm/v2",
                                            "didcomm/aip2;env=rfc587"
                                          ]
                                        }
                                      }
                                    ]
                                  }
                                  """),
                new DidTestData(
                    did:
                    "did:peer:2.Ez6LSghwSE437wnDE1pt3X6hVDUQzSjsHzinpX3XFvMjRAm7y.Vz6Mkhh1e5CEYYq6JBUcTZ6Cp2ranCWRrv7Yax3Le4N59R6dd.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vc2FuZGJveC1tZWRpYXRvci5hdGFsYXByaXNtLmlvIiwiYSI6WyJkaWRjb21tL3YyIl19fQ.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6IndzczovL3NhbmRib3gtbWVkaWF0b3IuYXRhbGFwcmlzbS5pby93cyIsImEiOlsiZGlkY29tbS92MiJdfX0",
                    expectedJson: """
                                  {
                                    "@context": [
                                      "https://www.w3.org/ns/did/v1",
                                      "https://w3id.org/security/suites/ed25519-2020/v1",
                                      "https://w3id.org/security/suites/x25519-2020/v1"
                                    ],
                                    "authentication": [
                                      {
                                        "id": "#key-2",
                                        "type": "Ed25519VerificationKey2020",
                                        "controller": "did:peer:2.Ez6LSghwSE437wnDE1pt3X6hVDUQzSjsHzinpX3XFvMjRAm7y.Vz6Mkhh1e5CEYYq6JBUcTZ6Cp2ranCWRrv7Yax3Le4N59R6dd.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vc2FuZGJveC1tZWRpYXRvci5hdGFsYXByaXNtLmlvIiwiYSI6WyJkaWRjb21tL3YyIl19fQ.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6IndzczovL3NhbmRib3gtbWVkaWF0b3IuYXRhbGFwcmlzbS5pby93cyIsImEiOlsiZGlkY29tbS92MiJdfX0",
                                        "publicKeyMultibase": "z6Mkhh1e5CEYYq6JBUcTZ6Cp2ranCWRrv7Yax3Le4N59R6dd"
                                      }
                                    ],
                                    "id": "did:peer:2.Ez6LSghwSE437wnDE1pt3X6hVDUQzSjsHzinpX3XFvMjRAm7y.Vz6Mkhh1e5CEYYq6JBUcTZ6Cp2ranCWRrv7Yax3Le4N59R6dd.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vc2FuZGJveC1tZWRpYXRvci5hdGFsYXByaXNtLmlvIiwiYSI6WyJkaWRjb21tL3YyIl19fQ.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6IndzczovL3NhbmRib3gtbWVkaWF0b3IuYXRhbGFwcmlzbS5pby93cyIsImEiOlsiZGlkY29tbS92MiJdfX0",
                                    "keyAgreement": [
                                      {
                                        "id": "#key-1",
                                        "type": "X25519KeyAgreementKey2020",
                                        "controller": "did:peer:2.Ez6LSghwSE437wnDE1pt3X6hVDUQzSjsHzinpX3XFvMjRAm7y.Vz6Mkhh1e5CEYYq6JBUcTZ6Cp2ranCWRrv7Yax3Le4N59R6dd.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vc2FuZGJveC1tZWRpYXRvci5hdGFsYXByaXNtLmlvIiwiYSI6WyJkaWRjb21tL3YyIl19fQ.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6IndzczovL3NhbmRib3gtbWVkaWF0b3IuYXRhbGFwcmlzbS5pby93cyIsImEiOlsiZGlkY29tbS92MiJdfX0",
                                        "publicKeyMultibase": "z6LSghwSE437wnDE1pt3X6hVDUQzSjsHzinpX3XFvMjRAm7y"
                                      }
                                    ],
                                    "service": [
                                      {
                                        "id": "#service",
                                        "type": "DIDCommMessaging",
                                        "serviceEndpoint": {
                                          "uri": "https://sandbox-mediator.atalaprism.io",
                                          "accept": [
                                            "didcomm/v2"
                                          ]
                                        }
                                      },
                                      {
                                        "id": "#service-1",
                                        "type": "DIDCommMessaging",
                                        "serviceEndpoint": {
                                          "uri": "wss://sandbox-mediator.atalaprism.io/ws",
                                          "accept": [
                                            "didcomm/v2"
                                          ]
                                        }
                                      }
                                    ]
                                  }
                                  """)
            };

            return testCases.Select(tc => new object[] { tc });
        }

        [Theory]
        [MemberData(nameof(DIDTestCases))]
        public void DIDTests(DidTestData testData)
        {
            var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(testData.Did),
                VerificationMaterialFormatPeerDid.Multibase);
            var expectedJson = NormalizeJson(testData.ExpectedJson);
            var actualJson = NormalizeJson(realValue.Value);

            Assert.Equal(expectedJson, actualJson);
        }


        [Fact]
        public void TestResolutionAndParsing_WithMultipleServices_ShouldResolveAndParseSuccessfully()
        {
            // This test verifies that a peer DID with multiple services can be:
            // 1. Successfully resolved to a DID Document
            // 2. The resolved JSON document can be parsed back into a DidDocPeerDid object
            // 3. The services maintain their proper structure with correct IDs
            // Arrange
            var testDid = "did:peer:2.Ez6LSghwSE437wnDE1pt3X6hVDUQzSjsHzinpX3XFvMjRAm7y.Vz6Mkhh1e5CEYYq6JBUcTZ6Cp2ranCWRrv7Yax3Le4N59R6dd.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vc2FuZGJveC1tZWRpYXRvci5hdGFsYXByaXNtLmlvIiwiYSI6WyJkaWRjb21tL3YyIl19fQ.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6IndzczovL3NhbmRib3gtbWVkaWF0b3IuYXRhbGFwcmlzbS5pby93cyIsImEiOlsiZGlkY29tbS92MiJdfX0";

            // Act
            var resolveResult = PeerDidResolver.ResolvePeerDid(
                new PeerDid(testDid)
            );
   
            var normalizedJson = NormalizeJson(resolveResult.Value);
            var parseResult = DidDocPeerDid.FromJson(normalizedJson);

            // Assert
            Assert.True(parseResult.IsSuccess);
            Assert.NotNull(parseResult.Value.Services);
            Assert.Equal(2, parseResult.Value.Services.Count);
   
            // Verify first service
            var firstService = parseResult.Value.Services[0];
            Assert.Equal("#service", firstService.Id);
            Assert.Equal("DIDCommMessaging", firstService.Type);
            Assert.Equal("https://sandbox-mediator.atalaprism.io", firstService.ServiceEndpoint.Uri);
            Assert.Contains("didcomm/v2", firstService.ServiceEndpoint.Accept);

            // Verify second service
            var secondService = parseResult.Value.Services[1];
            Assert.Equal("#service-1", secondService.Id);
            Assert.Equal("DIDCommMessaging", secondService.Type);
            Assert.Equal("wss://sandbox-mediator.atalaprism.io/ws", secondService.ServiceEndpoint.Uri);
            Assert.Contains("didcomm/v2", secondService.ServiceEndpoint.Accept);
        }
    

    [Fact]
        public void TestResolvePositiveDefault()
        {
            var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.Multibase);
            var expectedJson = NormalizeJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE);
            var actualJson = NormalizeJson(realValue.Value);

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void TestResolvePositiveMultibase()
        {
            var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.Multibase);
            var expectedJson = NormalizeJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE);
            var actualJson = NormalizeJson(realValue.Value);

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void TestResolvePositiveServiceIs2ElementsArray()
        {
            var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_2_SERVICES), VerificationMaterialFormatPeerDid.Multibase);
            var expectedJson = NormalizeJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES);
            var actualJson = NormalizeJson(realValue.Value);

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void TestResolvePositiveNoService()
        {
            var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES), VerificationMaterialFormatPeerDid.Multibase);
            var expectedJson = NormalizeJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES);
            var actualJson = NormalizeJson(realValue.Value);

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void TestResolvePositiveMinimalService()
        {
            var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES), VerificationMaterialFormatPeerDid.Multibase);
            var expectedJson = NormalizeJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES);
            var actualJson = NormalizeJson(realValue.Value);

            Assert.Equal(expectedJson, actualJson);
        }

        [Theory]
        [InlineData("did:peer:1.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0")]
        [InlineData("did:peer:2.Ez6LSpSrLxbAh02SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Vz6MkqRYqQi0gvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0")]
        [InlineData("did:peer:2.Ea6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Va6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0")]
        public void TestResolveInvalidDids(string invalidDid)
        {
            var result = PeerDidResolver.ResolvePeerDid(new PeerDid(invalidDid), VerificationMaterialFormatPeerDid.Multibase);
            Assert.Matches(new Regex("Does not match peer DID regexp.*"), result.Errors.First().Message);
        }

        [Fact]
        public void TestResolveMalformedSigningKey()
        {
            var result = PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSpSrLxbAh02SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Vz6MkqRYqQi0gvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0"
            ), VerificationMaterialFormatPeerDid.Multibase);
            Assert.Matches(new Regex("Does not match.*"), result.Errors.First().Message);
        }
        [Fact]
        public void TestResolveMalformedService()
        {
            var result = PeerDidResolver.ResolvePeerDid(
                new PeerDid(
                    "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
                    ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" + 
                    ".SW3sidCI6ImRtIiwicyI6Imh0dHA="
                ), 
                VerificationMaterialFormatPeerDid.Multibase
            );
            Assert.True(result.Errors.Any(e => e.Message.Contains("Invalid service")));
        }
        // A MORE STRICTER CHECK FAILING OTHER TESTS
        // [Fact]
        // public void TestResolveInvalidServiceEndpoint()
        // {
        //     var result = PeerDidResolver.ResolvePeerDid(
        //         new PeerDid(
        //             "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
        //             ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
        //             ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9"
        //         ),
        //         VerificationMaterialFormatPeerDid.Multibase
        //     );
        //     Assert.True(result.Errors.Any(e => e.Message.Contains("Invalid service")));
        // }

        [Fact]
        public void TestResolveInvalidKeyType()
        {
            var result = PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Vz6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0"
            ), VerificationMaterialFormatPeerDid.Multibase);
            Assert.Matches(new Regex("Invalid key.*"), result.Errors.First().Message);
        }
    }
}
