namespace Blocktrust.PeerDID.Tests;

using System.Text.Json;
using DIDDoc;
using Exceptions;
using Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class DidDocFromJson
{
    public static IEnumerable<object[]> DidDocNumalgo0()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_O_BASE58),
            VerificationMaterialFormatPeerDid.BASE58,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
            PublicKeyFieldValues.BASE58
        ));
        
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_O_MULTIBASE),
            VerificationMaterialFormatPeerDid.MULTIBASE,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
            PublicKeyFieldValues.MULTIBASE
        ));

        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_O_JWK),
            VerificationMaterialFormatPeerDid.JWK,
            VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
            VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
            PublicKeyFieldValues.JWK
        ));
        return list.Select(x => new object[] { x });
    }

    public static IEnumerable<object[]> DidDocNumalgo2()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_BASE58),
            VerificationMaterialFormatPeerDid.BASE58,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
            PublicKeyFieldValues.BASE58
        ));
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_MULTIBASE),
            VerificationMaterialFormatPeerDid.MULTIBASE,
            VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
            PublicKeyFieldValues.MULTIBASE
        ));
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_JWK),
            VerificationMaterialFormatPeerDid.JWK,
            VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
            VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
            PublicKeyFieldValues.JWK
        ));
        return list.Select(x => new object[] { x });
    }


    [Theory]
    [MemberData(nameof(DidDocNumalgo0))]
    public void TestDidDocFromJsonNumalgo0(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, didDoc.Did);

        Assert.True(didDoc.KeyAgreement.Count == 0);
        Assert.Null(didDoc.Service);
        Assert.Single(didDoc.Authentication);

        var auth = didDoc.Authentication[0];
        var expectedAuthArray = ((JsonElement)(Fixture.FromJson(testData.DidDoc.Value)["authentication"])).EnumerateArray().First();
        var expectedAuth = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedAuthArray.GetRawText());
        var expectedAuthId = ((JsonElement)expectedAuth["id"]).GetString();
        Assert.Equal(expectedAuthId, auth.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, auth.Controller);
        Assert.Equal(testData.ExpectedFormat, auth.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth.VerMaterial.Type);
        var expectedAuthField = ((JsonElement)expectedAuth[testData.ExpectedField]);
        if (expectedAuthField.ValueKind == JsonValueKind.String)
        {
            Assert.Equivalent(expectedAuthField.GetString(), auth.VerMaterial.Value);
        }
        else
        {
            var exptectedJwk = JsonSerializer.Deserialize<PeerDidJwk>(expectedAuthField);
            var computedJwk = (PeerDidJwk)auth.VerMaterial.Value;
            Assert.Equal(exptectedJwk!.Crv, computedJwk.Crv);
            Assert.Equal(exptectedJwk.Kty, computedJwk.Kty);
            Assert.Equal(exptectedJwk.X, computedJwk.X);
        }

        Assert.Equal(new List<object> { expectedAuthId }, didDoc.AuthenticationKids);
        Assert.True(didDoc.AgreementKids.Count == 0);
    }

    [Theory]
    [MemberData(nameof(DidDocNumalgo2))]
    public void TestDidDocFromJsonNumalgo2(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);

        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, didDoc.Did);

        Assert.Equal(2, didDoc.Authentication.Count);
        Assert.Single(didDoc.KeyAgreement);
        Assert.NotNull(didDoc.Service);
        Assert.Equal(1, didDoc.Service?.Count);

        var auth1 = didDoc.Authentication[0];
        var expectedAuthArray1 = ((JsonElement)(Fixture.FromJson(testData.DidDoc.Value)["authentication"])).EnumerateArray().First();
        var expectedAuth1 = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedAuthArray1.GetRawText());
        var expectedAuth1Id = ((JsonElement)expectedAuth1["id"]).GetString();
        Assert.Equal(expectedAuth1Id, auth1.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth1.Controller);
        Assert.Equal(testData.ExpectedFormat, auth1.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth1.VerMaterial.Type);
        var expectedAuthField1 = ((JsonElement)expectedAuth1[testData.ExpectedField]);
        if (expectedAuthField1.ValueKind == JsonValueKind.String)
        {
            Assert.Equivalent(expectedAuthField1.GetString(), auth1.VerMaterial.Value);
        }
        else
        {
            var exptectedJwk = JsonSerializer.Deserialize<PeerDidJwk>(expectedAuthField1);
            var computedJwk = (PeerDidJwk)auth1.VerMaterial.Value;
            Assert.Equal(exptectedJwk!.Crv, computedJwk.Crv);
            Assert.Equal(exptectedJwk.Kty, computedJwk.Kty);
            Assert.Equal(exptectedJwk.X, computedJwk.X);
        }
        
        var auth2 = didDoc.Authentication[1];
        var expectedAuthArray2 = ((JsonElement)(Fixture.FromJson(testData.DidDoc.Value)["authentication"])).EnumerateArray().Skip(1).First();
        var expectedAuth2 = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedAuthArray2.GetRawText());
        var expectedAuth2Id = ((JsonElement)expectedAuth2!["id"]).GetString();
        Assert.Equal(expectedAuth2Id, auth2.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, auth2.Controller);
        Assert.Equal(testData.ExpectedFormat, auth2.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAuthType, auth2.VerMaterial.Type);
        var expectedAuthField2 = ((JsonElement)expectedAuth2[testData.ExpectedField]);
        if (expectedAuthField2.ValueKind == JsonValueKind.String)
        {
            Assert.Equivalent(expectedAuthField2.GetString(), auth2.VerMaterial.Value);
        }
        else
        {
            var exptectedJwk = JsonSerializer.Deserialize<PeerDidJwk>(expectedAuthField2);
            var computedJwk = (PeerDidJwk)auth2.VerMaterial.Value;
            Assert.Equal(exptectedJwk!.Crv, computedJwk.Crv);
            Assert.Equal(exptectedJwk.Kty, computedJwk.Kty);
            Assert.Equal(exptectedJwk.X, computedJwk.X);
        }
        
        var agreem = didDoc.KeyAgreement[0];
        var expectedAgreemArray = ((JsonElement)(Fixture.FromJson(testData.DidDoc.Value)["keyAgreement"])).EnumerateArray().First();
        var expectedAgreem = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedAgreemArray.GetRawText());
        var expectedAgreemId = ((JsonElement)expectedAgreem!["id"]).GetString();
        Assert.Equal(expectedAgreemId, agreem.Id);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2, agreem.Controller);
        Assert.Equal(testData.ExpectedFormat, agreem.VerMaterial.Format);
        Assert.Equal(testData.ExpectedAgreemType, agreem.VerMaterial.Type);
        var expectedAgreemField = ((JsonElement)expectedAgreem[testData.ExpectedField]);
        if (expectedAgreemField.ValueKind == JsonValueKind.String)
        {
            Assert.Equivalent(expectedAgreemField.GetString(), agreem.VerMaterial.Value);
        }
        else
        {
            var exptectedJwk = JsonSerializer.Deserialize<PeerDidJwk>(expectedAgreemField);
            var computedJwk = (PeerDidJwk)agreem.VerMaterial.Value;
            Assert.Equal(exptectedJwk.Crv, computedJwk.Crv);
            Assert.Equal(exptectedJwk.Kty, computedJwk.Kty);
            Assert.Equal(exptectedJwk.X, computedJwk.X);
        }
        
        var service = didDoc.Service![0];
        var expectedServiceArray = ((JsonElement)(Fixture.FromJson(testData.DidDoc.Value)["service"])).EnumerateArray().First();
        var expectedService = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedServiceArray.GetRawText());
        var didCommService = (OtherService) service;
        Assert.Equivalent(expectedService!["id"].ToString(), didCommService.Data["id"].ToString());
        Assert.Equal(expectedService["serviceEndpoint"].ToString(), didCommService.Data["serviceEndpoint"].ToString());
        Assert.Equal(expectedService["type"].ToString(), didCommService.Data["type"].ToString());
        Assert.Equal(Fixture.RemoveWhiteSpace(expectedService!["routingKeys"].ToString()), Fixture.RemoveWhiteSpace(didCommService!.Data["routingKeys"].ToString()));
        Assert.Equal(Fixture.RemoveWhiteSpace(expectedService!["accept"].ToString()),Fixture.RemoveWhiteSpace( didCommService!.Data["accept"].ToString()));
        Assert.Equivalent(expectedAuth1["id"].ToString(), didDoc.AuthenticationKids[0]);
        Assert.Equivalent(expectedAuth2["id"].ToString(), didDoc.AuthenticationKids[1]);
        Assert.Equivalent( expectedAgreem["id"].ToString(), didDoc.AgreementKids[0]);
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2Service2Elements()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_2_SERVICES, didDoc.Did);

        Assert.NotNull(didDoc.Service);
        Assert.Equal(2, didDoc.Service.Count);

        var service1 = didDoc.Service[0];
        var expectedServiceArray1 = ((JsonElement)(Fixture.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"])).EnumerateArray().First();
        var expectedService1 = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedServiceArray1.GetRawText()); 
        var didCommService1 = (OtherService) service1;
        Assert.Equal(expectedService1!["id"].ToString(), didCommService1.Data["id"].ToString());
        Assert.Equal(expectedService1["serviceEndpoint"].ToString(),didCommService1.Data["serviceEndpoint"].ToString());
        Assert.Equal(expectedService1["type"].ToString(),didCommService1.Data["type"].ToString());
        Assert.Equal(Fixture.RemoveWhiteSpace(expectedService1!["routingKeys"].ToString()), Fixture.RemoveWhiteSpace(didCommService1!.Data["routingKeys"].ToString()));
        Assert.False(didCommService1.Data.ContainsKey("accept"));

        var service2 = didDoc.Service[1];
        var expectedServiceArray2 = ((JsonElement)(Fixture.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"])).EnumerateArray().Skip(1).First();
        var expectedService2 = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedServiceArray2.GetRawText()); 
        var didCommService2 = (OtherService) service2;
        Assert.Equal(Fixture.RemoveWhiteSpace(expectedService2!["accept"].ToString()),Fixture.RemoveWhiteSpace( didCommService2!.Data["accept"].ToString()));
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2NoService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES, didDoc.Did);
        Assert.Null(didDoc.Service);
        Assert.Single(didDoc.Authentication);
        Assert.Single(didDoc.KeyAgreement);
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2MinimalService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES, didDoc.Did);

        Assert.Equal(2, didDoc.Authentication.Count);
        Assert.Single(didDoc.KeyAgreement);

        var service = didDoc.Service![0];
        var didCommService = (OtherService)service;
        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg.SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9#didcommmessaging-0",
            didCommService.Data["id"].ToString()
        );
        Assert.Equal("https://example.com/endpoint", didCommService.Data["serviceEndpoint"].ToString());
        Assert.Equal("DIDCommMessaging", didCommService.Data["type"].ToString());
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
    public void TestDidDocInvalidJsonVerMethodNoId()
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
    public void TestDidDocInvalidJsonVerMethodNoType()
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
    public void TestDidDocInvalidJsonVerMethodNoController()
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
    public void TestDidDocInvalidJsonVerMethodNoValue()
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
    public void TestDidDocInvalidJsonVerMethodInvalidType()
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
    public void TestDidDocInvalidJsonVerMethodInvalidField()
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
    public void TestDidDocInvalidJsonVerMethodInvalidValueJwk()
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