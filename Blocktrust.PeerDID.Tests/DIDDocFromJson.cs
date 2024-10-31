using Blocktrust.Common.Models.DidDoc;
using Blocktrust.PeerDID.DIDDoc;
using Blocktrust.PeerDID.Types;

namespace Blocktrust.PeerDID.Tests;

public class DidDocFromJson
{
    public static IEnumerable<object[]> DidDocNumalgo0()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_O_MULTIBASE),
            VerificationMaterialFormatPeerDid.Multibase,
            VerificationMethodTypeAuthentication.Ed25519VerificationKey2020,
            VerificationMethodTypeAgreement.X25519KeyAgreementKey2020,
            PublicKeyFieldValues.Multibase
        ));
        return list.Select(x => new object[] { x });
    }

    public static IEnumerable<object[]> DidDocNumalgo2()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_MULTIBASE),
            VerificationMaterialFormatPeerDid.Multibase,
            VerificationMethodTypeAuthentication.Ed25519VerificationKey2020,
            VerificationMethodTypeAgreement.X25519KeyAgreementKey2020,
            PublicKeyFieldValues.Multibase
        ));
        return list.Select(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(DidDocNumalgo0))]
    public void TestDidDocFromJsonNumalgo0(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, didDoc.Value.Did);

        // Change this assertion to match absolute ID
        var auth = didDoc.Value.Authentications[0];
        Assert.Equal($"{Fixture.PEER_DID_NUMALGO_0}#key-1", auth.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, auth.Controller);
        Assert.Equal(testData.ExpectedFormat, auth.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth.VerMaterial.Type);

        // Update AuthenticationKids to use absolute IDs
        Assert.Equal(new List<object> { $"{Fixture.PEER_DID_NUMALGO_0}#key-1" }, didDoc.Value.AuthenticationKids);
    }

  
    [Theory]
    [MemberData(nameof(DidDocNumalgo2))]
    public void TestDidDocFromJsonNumalgo2(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, didDoc.Value.Did);
        Assert.Equal(2, didDoc.Value.Authentications.Count);
        Assert.Single(didDoc.Value.KeyAgreements);
        Assert.NotNull(didDoc.Value.Services);
        Assert.Single(didDoc.Value.Services);

        var auth1 = didDoc.Value.Authentications[0];
        Assert.Equal($"{Fixture.PEER_DID_NUMALGO_2}#key-2", auth1.Id); 
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth1.Controller);
        Assert.Equal(testData.ExpectedFormat, auth1.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth1.VerMaterial.Type);

        var auth2 = didDoc.Value.Authentications[1];
        Assert.Equal($"{Fixture.PEER_DID_NUMALGO_2}#key-3", auth2.Id); 
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth2.Controller);

        var keyAgreement = didDoc.Value.KeyAgreements[0];
        Assert.Equal($"{Fixture.PEER_DID_NUMALGO_2}#key-1", keyAgreement.Id); 
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, keyAgreement.Controller);

        var service = (Service)didDoc.Value.Services[0];
        Assert.Equal("DIDCommMessaging", service.Type);
        Assert.NotNull(service.ServiceEndpoint);
        Assert.Equal("https://example.com/endpoint", service.ServiceEndpoint.Uri);
        Assert.Contains("did:example:somemediator#somekey", service.ServiceEndpoint.RoutingKeys);
        Assert.Contains("didcomm/v2", service.ServiceEndpoint.Accept);
    }
    
    [Fact]
    public void TestDidDocFromJsonNumalgo2Service2Elements()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES);

        var service1 = (Service)didDoc.Value.Services[0];
        Assert.Equal("#service", service1.Id); // Changed to expect relative ID

        var service2 = (Service)didDoc.Value.Services[1];
        Assert.Equal("#service-1", service2.Id); // Changed to expect relative ID
    }
    [Fact]
    public void TestDidDocFromJsonNumalgo2NoService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES, didDoc.Value.Did);
        Assert.Null(didDoc.Value.Services);
        Assert.Single(didDoc.Value.Authentications);
        Assert.Single(didDoc.Value.KeyAgreements);
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2MinimalService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES, didDoc.Value.Did);

        Assert.Equal(2, didDoc.Value.Authentications.Count);
        Assert.Single(didDoc.Value.KeyAgreements);

        var service = (Service)didDoc.Value.Services![0];
        Assert.Equal("#service", service.Id);
        Assert.Equal("DIDCommMessaging", service.Type);
        Assert.Equal("https://example.com/endpoint", service.ServiceEndpoint.Uri);
        Assert.Empty(service.ServiceEndpoint.RoutingKeys ?? new List<string>());
        Assert.Empty(service.ServiceEndpoint.Accept ?? new List<string>());
    }

    [Fact]
    public void TestDidDocFromJsonInvalidJson()
    {
        var result = DidDocPeerDid.FromJson("invalid{json...");
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void TestDidDocIdFieldOnly()
    {
        var didDoc = DidDocPeerDid.FromJson("""
            {
                "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
            }
            """);
        Assert.Equal("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V", didDoc.Value.Did);
    }
    
    

    [Theory]
    [InlineData("No id field", """
                               {
                                   "authentication": [
                                       {
                                           "id": "#key-1",
                                           "type": "Ed25519VerificationKey2020",
                                           "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                                           "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                                       }
                                   ]
                               }
                               """)]

    //         // COMMENTED OUT, REASON: For did:peer:0/1 - We can generate an ID if not provided, using the format "#key-{incrementing number}"
    // [InlineData("No verification method id", """
    //     {
    //         "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
    //         "authentication": [
    //             {
    //                 "type": "Ed25519VerificationKey2020",
    //                 "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
    //                 "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
    //             }
    //         ]
    //     }
    //     """)]
    [InlineData("No verification method type", """
        {
            "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
            "authentication": [
                {
                    "id": "#key-1",
                    "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                    "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                }
            ]
        }
        """)]
    [InlineData("Invalid verification method type", """
        {
            "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
            "authentication": [
                {
                    "id": "#key-1",
                    "type": "InvalidType",
                    "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                    "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                }
            ]
        }
        """)]
    public void TestDidDocFromJsonInvalidDocuments(string scenario, string invalidDoc)
    {
        var result = DidDocPeerDid.FromJson(invalidDoc);
        Assert.False(result.IsSuccess);
    }
}