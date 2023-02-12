namespace Blocktrust.PeerDID.Tests;

using System.Text.RegularExpressions;
using PeerDID.Core;
using PeerDIDCreateResolve;
using Types;

public class CreateNumalgo2
{
    public static readonly VerificationMaterialAgreement VALID_X25519_KEY_BASE58 = new VerificationMaterialAgreement
    (
        value: "JhNWeSVLMYccCk7iopQW4guaSJTojqpMEELgSLhKwRr",
        type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
        format: VerificationMaterialFormatPeerDID.BASE58
    );

    public static readonly VerificationMaterialAgreement VALID_X25519_KEY_MULTIBASE = new VerificationMaterialAgreement
    (
        value: "z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc",
        type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
        format: VerificationMaterialFormatPeerDID.MULTIBASE
    );

    public static readonly VerificationMaterialAgreement VALID_X25519_KEY_JWK_DICT = new VerificationMaterialAgreement
    (
        value: new Dictionary<string, object>
        {
            { "kty", "OKP" },
            { "crv", "X25519" },
            { "x", "BIiFcQEn3dfvB2pjlhOQQour6jXy9d5s2FKEJNTOJik" },
        },
        type: VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
        format: VerificationMaterialFormatPeerDID.JWK
    );

    public static readonly VerificationMaterialAgreement VALID_X25519_KEY_JWK_JSON = new VerificationMaterialAgreement
    (
        value: Utils.ToJson(
            new Dictionary<string, object>
            {
                { "kty", "OKP" },
                { "crv", "X25519" },
                { "x", "BIiFcQEn3dfvB2pjlhOQQour6jXy9d5s2FKEJNTOJik" },
            }
        ),
        type: VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
        format: VerificationMaterialFormatPeerDID.JWK
    );

    public static readonly VerificationMaterialAuthentication VALID_ED25519_KEY_1_BASE58 = new VerificationMaterialAuthentication
    (
        value: "ByHnpUCFb1vAfh9CFZ8ZkmUZguURW8nSw889hy6rD8L7",
        type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
        format: VerificationMaterialFormatPeerDID.BASE58
    );

    public static readonly VerificationMaterialAuthentication VALID_ED25519_KEY_1_MULTIBASE = new VerificationMaterialAuthentication
    (
        value: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
        type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
        format: VerificationMaterialFormatPeerDID.MULTIBASE
    );

    public static readonly VerificationMaterialAuthentication VALID_ED25519_KEY_1_JWK = new VerificationMaterialAuthentication
    (
        value: new Dictionary<string, object>
        {
            { "kty", "OKP" },
            { "crv", "Ed25519" },
            { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_YgmA" },
        },
        type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
        format: VerificationMaterialFormatPeerDID.JWK
    );

    public static readonly VerificationMaterialAuthentication VALID_ED25519_KEY_2_BASE58 = new VerificationMaterialAuthentication
    (
        value: "3M5RCDjPTWPkKSN3sxUmmMqHbmRPegYP1tjcKyrDbt9J",
        type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
        format: VerificationMaterialFormatPeerDID.BASE58
    );

    public static readonly VerificationMaterialAuthentication VALID_ED25519_KEY_2_MULTIBASE = new VerificationMaterialAuthentication
    (
        value: "z6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg",
        type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
        format: VerificationMaterialFormatPeerDID.MULTIBASE
    );

    public static readonly VerificationMaterialAuthentication VALID_ED25519_KEY_2_JWK = new VerificationMaterialAuthentication
    (
        value: new Dictionary<string, object>
        {
            { "kty", "OKP" },
            { "crv", "Ed25519" },
            { "x", "Itv8B__b1-Jos3LCpUe8EdTFGTCa_Dza6_3848P3R70" },
        },
        type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
        format: VerificationMaterialFormatPeerDID.JWK
    );

    public const string VALID_SERVICE = """
    {
        "type": "DIDCommMessaging",
        "serviceEndpoint": "https://example.com/endpoint",
        "routingKeys": ["did:example:somemediator#somekey"],
        "accept": ["didcomm/v2", "didcomm/aip2;env=rfc587"]
    }
    """;

    public static IEnumerable<object[]> ValidKeys()
    {
        var list = new List<TestData>();
        list.Add(new TestData
        {
            signingKeys = new List<VerificationMaterialAuthentication>
            {
                VALID_ED25519_KEY_1_BASE58,
                VALID_ED25519_KEY_2_BASE58
            },
            encKeys = new List<VerificationMaterialAgreement>
            {
                VALID_X25519_KEY_BASE58
            }
        });
        list.Add(new TestData
        {
            signingKeys = new List<VerificationMaterialAuthentication>
            {
                VALID_ED25519_KEY_1_MULTIBASE,
                VALID_ED25519_KEY_2_MULTIBASE
            },
            encKeys = new List<VerificationMaterialAgreement>
            {
                VALID_X25519_KEY_MULTIBASE
            }
        });

        list.Add(new TestData
        {
            signingKeys = new List<VerificationMaterialAuthentication>
            {
                VALID_ED25519_KEY_1_JWK,
                VALID_ED25519_KEY_2_JWK
            },
            encKeys = new List<VerificationMaterialAgreement>
            {
                VALID_X25519_KEY_JWK_DICT
            }
        });

        list.Add(new TestData
        {
            signingKeys = new List<VerificationMaterialAuthentication>
            {
                VALID_ED25519_KEY_1_JWK,
                VALID_ED25519_KEY_2_JWK
            },
            encKeys = new List<VerificationMaterialAgreement>
            {
                VALID_X25519_KEY_JWK_JSON
            }
        });
        return list.Select(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(ValidKeys))]
    public void TestCreateNumalgo2Positive(TestData keys)
    {
        string service = """
        [
        {
            "type": "DIDCommMessaging",
            "serviceEndpoint": "https://example.com/endpoint",
            "routingKeys": ["did:example:somemediator#somekey"]
        },
        {
            "type": "example",
            "serviceEndpoint": "https://example.com/endpoint2",
            "routingKeys": ["did:example:somemediator#somekey2"],
            "accept": ["didcomm/v2", "didcomm/aip2;env=rfc587"]
        }
        ]
        """;

        string peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(keys.encKeys, keys.signingKeys, service);

        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
            ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
            ".Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg" +
            ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il19LHsidCI6ImV4YW1wbGUiLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdLCJhIjpbImRpZGNvbW0vdjIiLCJkaWRjb21tL2FpcDI7ZW52PXJmYzU4NyJdfV0",
            peerDidAlgo2);

        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2PositiveServiceNotArray()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        string service = """
            {
                "type": "DIDCommMessaging",
                "serviceEndpoint": "https://example.com/endpoint",
                "routingKeys": ["did:example:somemediator#somekey"]
            }
            """;

        string peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service);

        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2PositiveServiceMinimalFields()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        string service = """
            {
                "type": "DIDCommMessaging",
                "serviceEndpoint": "https://example.com/endpoint"
            }
            """;

        string peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service) ?? throw new ArgumentNullException("PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service)");

        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2PositiveServiceArrayOf1Element()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };

        string service = @"
    [
        {
            ""type"": ""DIDCommMessaging"",
            ""serviceEndpoint"": ""https://example.com/endpoint"",
            ""routingKeys"": [""did:example:somemediator#somekey""]
        }       
    ]
    ";

        var peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service);

        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2PositiveServiceIsNull()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };

        string service = null;

        string peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service);

        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
            ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
            ".Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg",
            peerDidAlgo2
        );
        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void testCreateNumalgo2WithoutEncryptionKeys()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement>();
        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };

        string service = VALID_SERVICE;

        string peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service);
        Assert.Equal(
            "did:peer:2" +
            ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
            ".Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg" +
            ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0",
            peerDidAlgo2
        );
        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void testCreateNumalgo2EmptySigningKeys()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication>();
        string service = VALID_SERVICE;

        string peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service);
        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
            ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0",
            peerDidAlgo2
        );
        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2WrongEncryptionKey()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement>
        {
            new VerificationMaterialAgreement
            (
                value: "...",
                type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        var service = VALID_SERVICE;

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            PeerDIDCreator.CreatePeerDIDNumalgo2(
                encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                service: service
            );
        });
        Assert.Matches(new Regex("Invalid key: Invalid base58 encoding.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2WrongSigningKey()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "....",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "3M5RCDjPTWPkKSN3sxUmmMqHbmRPegYP1tjcKyrDbt9J",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var service = VALID_SERVICE;

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            PeerDIDCreator.CreatePeerDIDNumalgo2(
                encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                service: service
            );
        });
        Assert.Matches(new Regex("Invalid key: Invalid base58 encoding.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2WrongService()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        var service = """...""";
        var ex = Assert.Throws<ArgumentException>(() =>
        {
            PeerDIDCreator.CreatePeerDIDNumalgo2(
                encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                service: service
            );
        });
        Assert.Matches(new Regex("Invalid JSON.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2EncryptionKeysAndSigningAreMoreThan1ElementArray()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement>
        {
            VALID_X25519_KEY_MULTIBASE,
            VALID_X25519_KEY_JWK_DICT,
            VALID_X25519_KEY_BASE58
        };
        var signingKeys = new List<VerificationMaterialAuthentication>
        {
            VALID_ED25519_KEY_1_MULTIBASE,
            VALID_ED25519_KEY_2_MULTIBASE,
            VALID_ED25519_KEY_1_BASE58,
            VALID_ED25519_KEY_2_JWK
        };
        var service = VALID_SERVICE;

        var peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(
            encryptionKeys: encryptionKeys, signingKeys: signingKeys,
            service: service
        );
        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2ServiceHasMoreFieldsThanInConversionTable()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        var service = """
    {
        "type": "DIDCommMessaging",
        "serviceEndpoint": "https://example.com/endpoint",
        "routingKeys": ["did:example:somemediator#somekey"],
        "example1": "myExample1",
        "example2": "myExample2"
    }
    """;

        var peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(
            encryptionKeys: encryptionKeys, signingKeys: signingKeys,
            service: service
        ) ?? throw new ArgumentNullException("PeerDIDCreator.CreatePeerDIDNumalgo2(\n            encryptionKeys: encryptionKeys, signingKeys: signingKeys,\n            service: service\n        )");
        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2ServiceIsNotdidcommmessaging()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };

        var service = """
    {
        "type": "example1",
        "serviceEndpoint": "https://example.com/endpoint",
        "routingKeys": ["did:example:somemediator#somekey"]
    }
    """;

        var peerDidAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(
            encryptionKeys: encryptionKeys, signingKeys: signingKeys,
            service: service
        ) ?? throw new ArgumentNullException("PeerDIDCreator.CreatePeerDIDNumalgo2(\n            encryptionKeys: encryptionKeys, signingKeys: signingKeys,\n            service: service\n        )");
        Assert.True(PeerDIDCreator.IsPeerDID(peerDidAlgo2));
    }

    [Fact]
    public void TestCreateNumalgo2ServiceIsEmptyString()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        var service = "";

        Assert.True(
            PeerDIDCreator.IsPeerDID(
                PeerDIDCreator.CreatePeerDIDNumalgo2(
                    encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                    service: service
                )
            )
        );
    }

    [Fact]
    public void TestCreateNumalgo2MalformedEncryptionKeyNotBase58Encoded()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement>
        {
            new VerificationMaterialAgreement
            (
                value: "JhNWeSVLMYcc0k7iopQW4guaSJTojqpMEELgSLhKwRr",
                type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };

        var service = VALID_SERVICE;

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            PeerDIDCreator.CreatePeerDIDNumalgo2(
                encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                service: service
            );
        });
        Assert.Matches(new Regex("Invalid key: Invalid base58 encoding.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2MalformedSigningKeyNotBase58Encoded()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "ByHnpUCFb1vA0h9CFZ8ZkmUZguURW8nSw889hy6rD8L7",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "3M5RCDjPTWPkKSN3sxUmmMqHbmRPegYP1tjcKyrDbt9J",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var service = VALID_SERVICE;
        var ex = Assert.Throws<ArgumentException>(() =>
        {
            PeerDIDCreator.CreatePeerDIDNumalgo2(
                encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                service: service
            );
        });
        Assert.Matches(new Regex("Invalid key: Invalid base58 encoding.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2MalformedLongEncryptionKey()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement>
        {
            new VerificationMaterialAgreement
            (
                value: "JhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWe",
                type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };

        var service = VALID_SERVICE;

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            PeerDIDCreator.CreatePeerDIDNumalgo2(
                encryptionKeys: encryptionKeys, signingKeys: signingKeys,
                service: service
            );
        });
        Assert.Matches(new Regex("Invalid key.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2MalformedShortEncryptionKey()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement>
        {
            new VerificationMaterialAgreement
            (
                value: "JhNWeSV",
                type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var signingKeys = new List<VerificationMaterialAuthentication> { VALID_ED25519_KEY_1_MULTIBASE, VALID_ED25519_KEY_2_MULTIBASE };
        var service = VALID_SERVICE;

        var ex = Assert.Throws<ArgumentException>(() => PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service));
        Assert.Matches(new Regex("Invalid key.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2MalformedLongSigningKey()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "JhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWeSVJhNWe",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "3M5RCDjPTWPkKSN3sxUmmMqHbmRPegYP1tjcKyrDbt9J",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var service = VALID_SERVICE;

        var ex = Assert.Throws<ArgumentException>(() => PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service));
        Assert.Matches(new Regex("Invalid key.*"), ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo2MalformedShortSigningKey()
    {
        var encryptionKeys = new List<VerificationMaterialAgreement> { VALID_X25519_KEY_MULTIBASE };
        var signingKeys = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "JhNWeSV",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "3M5RCDjPTWPkKSN3sxUmmMqHbmRPegYP1tjcKyrDbt9J",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58
            )
        };
        var service = "VALID_SERVICE";
        var ex = Assert.Throws<ArgumentException>(() => PeerDIDCreator.CreatePeerDIDNumalgo2(encryptionKeys, signingKeys, service));
        Assert.Matches("Invalid key.*", ex.Message);
    }
}