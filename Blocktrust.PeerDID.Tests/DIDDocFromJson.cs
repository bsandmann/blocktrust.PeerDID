namespace Blocktrust.PeerDID.Tests;

using DIDDoc;
using Exceptions;
using Newtonsoft.Json;
using Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class DidDocFromJson
{
    public static IEnumerable<object[]> DidDocNumalgo0()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json("DID_DOC_NUMALGO_O_BASE58"),
            VerificationMaterialFormatPeerDid.BASE58,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
            PublicKeyFieldValues.BASE58
        ));

        list.Add(new DidDocTestData(
            new Json("DID_DOC_NUMALGO_O_MULTIBASE"),
            VerificationMaterialFormatPeerDid.MULTIBASE,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
            PublicKeyFieldValues.MULTIBASE
        ));

        list.Add(new DidDocTestData(
            new Json("DID_DOC_NUMALGO_O_JWK"),
            VerificationMaterialFormatPeerDid.JWK,
            VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
            VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
            PublicKeyFieldValues.JWK
        ));
        return list.Select(x => new object[] {x});
    }

    public static IEnumerable<object[]> DidDocNumalgo2()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json("DID_DOC_NUMALGO_2_BASE58"),
            VerificationMaterialFormatPeerDid.BASE58,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
            PublicKeyFieldValues.BASE58
        ));
        list.Add( new DidDocTestData(
            new Json("DID_DOC_NUMALGO_2_MULTIBASE"),
            VerificationMaterialFormatPeerDid.MULTIBASE,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
            PublicKeyFieldValues.MULTIBASE
        ));
        list.Add( new DidDocTestData(
            new Json( "DID_DOC_NUMALGO_2_JWK"),
            VerificationMaterialFormatPeerDid.JWK,
            VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
            VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
            PublicKeyFieldValues.JWK
        ));
        return list.Select(x => new object[] {x});
    }

    [Theory]
    [MemberData(nameof(DidDocNumalgo0))]
    public void TestDidDocFromJsonNumalgo0(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, didDoc.Did);

        Assert.True(didDoc.KeyAgreement.Count == 0);
        Assert.Null(didDoc.Service);
        Assert.Equal(1, didDoc.Authentication.Count);

        var auth = didDoc.Authentication[0];
        var expectedAuth = (Fixture.FromJson(testData.DidDoc.Value)["authentication"] as List<Dictionary<string, object>>)[0];
        Assert.Equal(expectedAuth["id"], auth.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, auth.Controller);
        Assert.Equal(testData.ExpectedFormat, auth.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth.VerMaterial.Type);
        Assert.Equal(expectedAuth[testData.ExpectedField], auth.VerMaterial.Value);

        Assert.Equal(new List<object> { expectedAuth["id"] }, didDoc.AuthenticationKids);
        Assert.True(didDoc.AgreementKids.Count == 0);
    }

    [Theory]
    [MemberData(nameof(DidDocNumalgo2))]
    public void TestDidDocFromJsonNumalgo2(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);

        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, didDoc.Did);

        Assert.Equal(2, didDoc.Authentication.Count);
        Assert.Equal(1, didDoc.KeyAgreement.Count);
        Assert.NotNull(didDoc.Service);
        Assert.Equal(1, didDoc.Service?.Count);

        var auth1 = didDoc.Authentication[0];
        var expectedAuth1 = ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value))["authentication"] as List<Dictionary<string, object>>)[0];
        Assert.Equal(expectedAuth1["id"], auth1.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth1.Controller);
        Assert.Equal(testData.ExpectedFormat, auth1.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth1.VerMaterial.Type);
        Assert.Equal(expectedAuth1[testData.ExpectedField], auth1.VerMaterial.Value);

        var auth2 = didDoc.Authentication[1];
        var expectedAuth2 =  ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value))["authentication"] as List<Dictionary<string, object>>)[1];
        Assert.Equal(expectedAuth2["id"], auth2.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth2.Controller);
        Assert.Equal(testData.ExpectedFormat, auth2.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth2.VerMaterial.Type);
        Assert.Equal(expectedAuth2[testData.ExpectedField], auth2.VerMaterial.Value);

        var agreem = didDoc.KeyAgreement[0];
        var expectedAgreem =  ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value))["keyAgreement"] as List<Dictionary<string, object>>)[0];
        Assert.Equal(expectedAgreem["id"], agreem.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, agreem.Controller);
        Assert.Equal(testData.ExpectedFormat, agreem.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAgreemType, agreem.VerMaterial.Type);
        Assert.Equal(expectedAgreem[testData.ExpectedField], agreem.VerMaterial.Value);

        var service = didDoc.Service[0];
        var expectedService = ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value)["service"]) as List<Dictionary<string, object>>)[0];
        Assert.IsAssignableFrom<DidCommServicePeerDid>(service);
        var didCommService = service as DidCommServicePeerDid;
        Assert.Equal(expectedService["id"], didCommService.Id);
        Assert.Equal(expectedService["serviceEndpoint"], didCommService.ServiceEndpoint);
        Assert.Equal(expectedService["type"], didCommService.Type);
        Assert.Equal(expectedService["routingKeys"], didCommService.RoutingKeys);
        Assert.Equal(expectedService["accept"], didCommService.Accept);

        Assert.Equal(new List<object> { expectedAuth1["id"], expectedAuth2["id"] }, didDoc.AuthenticationKids);
        Assert.Equal(new List<object> { expectedAgreem["id"] }, didDoc.AgreementKids);
    }
    
    [Fact]
    public void TestDidDocFromJsonNumalgo2Service2Elements()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_2_SERVICES, didDoc.Did);

        Assert.NotNull(didDoc.Service);
        Assert.Equal(2, didDoc.Service.Count);

        var service1 = didDoc.Service[0];
        var expectedService1 = (JsonConvert.DeserializeObject<Dictionary<string, object>>(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"] as List<Dictionary<string, object>>)[0];
        Assert.IsType<DidCommServicePeerDid>(service1);
        var didCommService1 = service1 as DidCommServicePeerDid;
        Assert.Equal(expectedService1["id"], didCommService1.Id);
        Assert.Equal(expectedService1["serviceEndpoint"], didCommService1.ServiceEndpoint);
        Assert.Equal(expectedService1["type"], didCommService1.Type);
        Assert.Equal(expectedService1["routingKeys"], didCommService1.RoutingKeys);
        Assert.Empty(didCommService1.Accept);

        var service2 = didDoc.Service[1];
        var expectedService2 = (JsonConvert.DeserializeObject<Dictionary<string, object>>(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"] as List<Dictionary<string, object>>)[1];
        Assert.IsType<OtherService>(service2);
        var didCommService2 = service2 as DidCommServicePeerDid;
        Assert.Equal(expectedService2, didCommService2.ToDict());
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2NoService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES, didDoc.Did);
        Assert.Null(didDoc.Service);
        Assert.Equal(1, didDoc.Authentication.Count);
        Assert.Equal(1, didDoc.KeyAgreement.Count);
    }
    
    [Fact]
    public void testDidDocFromJsonNumalgo2MinimalService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES, didDoc.Did);

        Assert.Equal(2, didDoc.Authentication.Count);
        Assert.Equal(1, didDoc.KeyAgreement.Count);

        var service = didDoc.Service[0];
        Assert.True(service is DidCommServicePeerDid);
         var didCommService = service as DidCommServicePeerDid;
        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg.SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9#didcommmessaging-0",
            didCommService.Id
        );
        Assert.Equal("https://example.com/endpoint", didCommService.ServiceEndpoint);
        Assert.Equal("DIDCommMessaging", didCommService.Type);
        Assert.True(!didCommService.RoutingKeys.Any());
        Assert.True(!didCommService.Accept.Any()); 
    }
    
    [Fact]
    public void TestDidDocFromJsonInvalidJson()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() => DidDocPeerDid.FromJson("sdfasdfsf{sdfsdfasdf..."));
    }

    [Fact]
    public void TestDidDocIdFieldOnly()
    {
        var didDoc = DidDocPeerDid.FromJson(
            @"
        {
            ""id"": ""did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V""
        }");
        Assert.Equal("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V", didDoc.Did);
    }
    
    [Fact]
    public void TestDidDocInvalidJsonNoId()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "type": "Ed25519VerificationKey2020",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodNoId()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "type": "Ed25519VerificationKey2020",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodNoType()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodNoController()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "type": "Ed25519VerificationKey2020",
                               "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodNoValue()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "type": "Ed25519VerificationKey2020",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodInvalidType()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "type": "Unkknown",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "publicKeyMultibase": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodInvalidField()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "type": "Ed25519VerificationKey2020",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "publicKeyJwk": "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                           }
                       ]
                   }
            """);
        });
    }
    
    [Fact]
    public void testDidDocInvalidJsonVerMethodInvalidValueJwk()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DidDocPeerDid.FromJson("""
                   {
                       "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                       "authentication": [
                           {
                               "id": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V#6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "type": "JsonWebKey2020",
                               "controller": "did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                               "publicKeyJwk": "sdfsdf{sfsdfdf"
                           }
                       ]
                   }
            """);
        });
    }

}