namespace Blocktrust.PeerDID.Tests;

using DIDDoc;
using Exceptions;
using Newtonsoft.Json;
using Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class DIDDocFromJson
{
    public static IEnumerable<object[]> DidDocNumalgo0()
    {
        var list = new List<DIDDocTestData>();
        list.Add(new DIDDocTestData(
            new JSON("DID_DOC_NUMALGO_O_BASE58"),
            VerificationMaterialFormatPeerDID.BASE58,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
            PublicKeyFieldValues.BASE58
        ));

        list.Add(new DIDDocTestData(
            new JSON("DID_DOC_NUMALGO_O_MULTIBASE"),
            VerificationMaterialFormatPeerDID.MULTIBASE,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
            PublicKeyFieldValues.MULTIBASE
        ));

        list.Add(new DIDDocTestData(
            new JSON("DID_DOC_NUMALGO_O_JWK"),
            VerificationMaterialFormatPeerDID.JWK,
            VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
            VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
            PublicKeyFieldValues.JWK
        ));
        return list.Select(x => new object[] {x});
    }

    public static IEnumerable<object[]> DidDocNumalgo2()
    {
        var list = new List<DIDDocTestData>();
        list.Add(new DIDDocTestData(
            new JSON("DID_DOC_NUMALGO_2_BASE58"),
            VerificationMaterialFormatPeerDID.BASE58,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
            PublicKeyFieldValues.BASE58
        ));
        list.Add( new DIDDocTestData(
            new JSON("DID_DOC_NUMALGO_2_MULTIBASE"),
            VerificationMaterialFormatPeerDID.MULTIBASE,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
            PublicKeyFieldValues.MULTIBASE
        ));
        list.Add( new DIDDocTestData(
            new JSON( "DID_DOC_NUMALGO_2_JWK"),
            VerificationMaterialFormatPeerDID.JWK,
            VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
            VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
            PublicKeyFieldValues.JWK
        ));
        return list.Select(x => new object[] {x});
    }

    [Theory]
    [MemberData(nameof(DidDocNumalgo0))]
    public void TestDidDocFromJsonNumalgo0(DIDDocTestData testData)
    {
        var didDoc = DIDDocPeerDID.fromJson(testData.DidDoc.Value);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, didDoc.did);

        Assert.True(didDoc.keyAgreement.Count == 0);
        Assert.Null(didDoc.service);
        Assert.Equal(1, didDoc.authentication.Count);

        var auth = didDoc.authentication[0];
        var expectedAuth = (Fixture.FromJson(testData.DidDoc.Value)["authentication"] as List<Dictionary<string, object>>)[0];
        Assert.Equal(expectedAuth["id"], auth.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, auth.Controller);
        Assert.Equal(testData.ExpectedFormat, auth.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth.VerMaterial.Type);
        Assert.Equal(expectedAuth[testData.ExpectedField], auth.VerMaterial.Value);

        Assert.Equal(new List<object> { expectedAuth["id"] }, didDoc.authenticationKids);
        Assert.True(didDoc.agreementKids.Count == 0);
    }

    [Theory]
    [MemberData(nameof(DidDocNumalgo2))]
    public void TestDidDocFromJsonNumalgo2(DIDDocTestData testData)
    {
        var didDoc = DIDDocPeerDID.fromJson(testData.DidDoc.Value);

        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, didDoc.did);

        Assert.Equal(2, didDoc.authentication.Count);
        Assert.Equal(1, didDoc.keyAgreement.Count);
        Assert.NotNull(didDoc.service);
        Assert.Equal(1, didDoc.service?.Count);

        var auth1 = didDoc.authentication[0];
        var expectedAuth1 = ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value))["authentication"] as List<Dictionary<string, object>>)[0];
        Assert.Equal(expectedAuth1["id"], auth1.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth1.Controller);
        Assert.Equal(testData.ExpectedFormat, auth1.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth1.VerMaterial.Type);
        Assert.Equal(expectedAuth1[testData.ExpectedField], auth1.VerMaterial.Value);

        var auth2 = didDoc.authentication[1];
        var expectedAuth2 =  ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value))["authentication"] as List<Dictionary<string, object>>)[1];
        Assert.Equal(expectedAuth2["id"], auth2.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth2.Controller);
        Assert.Equal(testData.ExpectedFormat, auth2.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth2.VerMaterial.Type);
        Assert.Equal(expectedAuth2[testData.ExpectedField], auth2.VerMaterial.Value);

        var agreem = didDoc.keyAgreement[0];
        var expectedAgreem =  ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value))["keyAgreement"] as List<Dictionary<string, object>>)[0];
        Assert.Equal(expectedAgreem["id"], agreem.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, agreem.Controller);
        Assert.Equal(testData.ExpectedFormat, agreem.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAgreemType, agreem.VerMaterial.Type);
        Assert.Equal(expectedAgreem[testData.ExpectedField], agreem.VerMaterial.Value);

        var service = didDoc.service[0];
        var expectedService = ((JsonSerializer.Deserialize<Dictionary<string, object>>(testData.DidDoc.Value)["service"]) as List<Dictionary<string, object>>)[0];
        Assert.IsAssignableFrom<DIDCommServicePeerDID>(service);
        var didCommService = service as DIDCommServicePeerDID;
        Assert.Equal(expectedService["id"], didCommService.id);
        Assert.Equal(expectedService["serviceEndpoint"], didCommService.serviceEndpoint);
        Assert.Equal(expectedService["type"], didCommService.type);
        Assert.Equal(expectedService["routingKeys"], didCommService.routingKeys);
        Assert.Equal(expectedService["accept"], didCommService.accept);

        Assert.Equal(new List<object> { expectedAuth1["id"], expectedAuth2["id"] }, didDoc.authenticationKids);
        Assert.Equal(new List<object> { expectedAgreem["id"] }, didDoc.agreementKids);
    }
    
    [Fact]
    public void TestDidDocFromJsonNumalgo2Service2Elements()
    {
        var didDoc = DIDDocPeerDID.fromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_2_SERVICES, didDoc.did);

        Assert.NotNull(didDoc.service);
        Assert.Equal(2, didDoc.service.Count);

        var service1 = didDoc.service[0];
        var expectedService1 = (JsonConvert.DeserializeObject<Dictionary<string, object>>(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"] as List<Dictionary<string, object>>)[0];
        Assert.IsType<DIDCommServicePeerDID>(service1);
        var didCommService1 = service1 as DIDCommServicePeerDID;
        Assert.Equal(expectedService1["id"], didCommService1.id);
        Assert.Equal(expectedService1["serviceEndpoint"], didCommService1.serviceEndpoint);
        Assert.Equal(expectedService1["type"], didCommService1.type);
        Assert.Equal(expectedService1["routingKeys"], didCommService1.routingKeys);
        Assert.Empty(didCommService1.accept);

        var service2 = didDoc.service[1];
        var expectedService2 = (JsonConvert.DeserializeObject<Dictionary<string, object>>(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"] as List<Dictionary<string, object>>)[1];
        Assert.IsType<OtherService>(service2);
        var didCommService2 = service2 as DIDCommServicePeerDID;
        Assert.Equal(expectedService2, didCommService2.ToDict());
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2NoService()
    {
        var didDoc = DIDDocPeerDID.fromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES, didDoc.did);
        Assert.Null(didDoc.service);
        Assert.Equal(1, didDoc.authentication.Count);
        Assert.Equal(1, didDoc.keyAgreement.Count);
    }
    
    [Fact]
    public void testDidDocFromJsonNumalgo2MinimalService()
    {
        var didDoc = DIDDocPeerDID.fromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES, didDoc.did);

        Assert.Equal(2, didDoc.authentication.Count);
        Assert.Equal(1, didDoc.keyAgreement.Count);

        var service = didDoc.service[0];
        Assert.True(service is DIDCommServicePeerDID);
         var didCommService = service as DIDCommServicePeerDID;
        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg.SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9#didcommmessaging-0",
            didCommService.id
        );
        Assert.Equal("https://example.com/endpoint", didCommService.serviceEndpoint);
        Assert.Equal("DIDCommMessaging", didCommService.type);
        Assert.True(!didCommService.routingKeys.Any());
        Assert.True(!didCommService.accept.Any()); 
    }
    
    [Fact]
    public void TestDidDocFromJsonInvalidJson()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() => DIDDocPeerDID.fromJson("sdfasdfsf{sdfsdfasdf..."));
    }

    [Fact]
    public void TestDidDocIdFieldOnly()
    {
        var didDoc = DIDDocPeerDID.fromJson(
            @"
        {
            ""id"": ""did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V""
        }");
        Assert.Equal("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V", didDoc.did);
    }
    
    [Fact]
    public void TestDidDocInvalidJsonNoId()
    {
        Assert.Throws<MalformedPeerDIDDocException>(() =>
        {
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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
            DIDDocPeerDID.fromJson("""
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