namespace Blocktrust.PeerDID.Tests;

using System.Text.Json;
using Common.Models.DidDoc;
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
            VerificationMaterialFormatPeerDid.Base58,
            VerificationMethodTypeAuthentication.Ed25519VerificationKey2018,
            VerificationMethodTypeAgreement.X25519KeyAgreementKey2019,
            PublicKeyFieldValues.Base58
        ));

        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_O_MULTIBASE),
            VerificationMaterialFormatPeerDid.Multibase,
            VerificationMethodTypeAuthentication.Ed25519VerificationKey2020,
            VerificationMethodTypeAgreement.X25519KeyAgreementKey2020,
            PublicKeyFieldValues.Multibase
        ));

        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_O_JWK),
            VerificationMaterialFormatPeerDid.Jwk,
            VerificationMethodTypeAuthentication.JsonWebKey2020,
            VerificationMethodTypeAgreement.JsonWebKey2020,
            PublicKeyFieldValues.Jwk
        ));
        return list.Select(x => new object[] { x });
    }

    public static IEnumerable<object[]> DidDocNumalgo2()
    {
        var list = new List<DidDocTestData>();
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_BASE58),
            VerificationMaterialFormatPeerDid.Base58,
            VerificationMethodTypeAuthentication.Ed25519VerificationKey2018,
            VerificationMethodTypeAgreement.X25519KeyAgreementKey2019,
            PublicKeyFieldValues.Base58
        ));
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_MULTIBASE),
            VerificationMaterialFormatPeerDid.Multibase,
            VerificationMethodTypeAuthentication.Ed25519VerificationKey2020,
            VerificationMethodTypeAgreement.X25519KeyAgreementKey2020,
            PublicKeyFieldValues.Multibase
        ));
        list.Add(new DidDocTestData(
            new Json(Fixture.DID_DOC_NUMALGO_2_JWK),
            VerificationMaterialFormatPeerDid.Jwk,
            VerificationMethodTypeAuthentication.JsonWebKey2020,
            VerificationMethodTypeAgreement.JsonWebKey2020,
            PublicKeyFieldValues.Jwk
        ));
        return list.Select(x => new object[] { x });
    }


    [Theory]
    [MemberData(nameof(DidDocNumalgo0))]
    public void TestDidDocFromJsonNumalgo0(DidDocTestData testData)
    {
        var didDoc = DidDocPeerDid.FromJson(testData.DidDoc.Value);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_0, didDoc.Did);

        Assert.True(didDoc.KeyAgreements.Count == 0);
        Assert.Null(didDoc.Services);
        Assert.Single(didDoc.Authentications);

        var auth = didDoc.Authentications[0];
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

        Assert.Equal(2, didDoc.Authentications.Count);
        Assert.Single(didDoc.KeyAgreements);
        Assert.NotNull(didDoc.Services);
        Assert.Equal(1, didDoc.Services?.Count);

        var auth1 = didDoc.Authentications[0];
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

        var auth2 = didDoc.Authentications[1];
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

        var agreem = didDoc.KeyAgreements[0];
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

        var service = didDoc.Services![0];
        var expectedServiceArray = ((JsonElement)(Fixture.FromJson(testData.DidDoc.Value)["service"])).EnumerateArray().First();
        var expectedService = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedServiceArray.GetRawText());
        var didCommService = (Service)service;
        Assert.Equivalent(expectedService!["id"].ToString(), didCommService.Id);
        Assert.Equal(expectedService["serviceEndpoint"].ToString(), didCommService.ServiceEndpoint);
        Assert.Equal(expectedService["type"].ToString(), didCommService.Type);
        Assert.Equivalent(((JsonElement)expectedService!["routingKeys"]).EnumerateArray().Select(p => p.GetString()).ToList(), didCommService!.RoutingKeys);
        Assert.Equivalent(((JsonElement)expectedService!["accept"]).EnumerateArray().Select(p => p.GetString()).ToList(), didCommService!.Accept);
        Assert.Equivalent(expectedAuth1["id"].ToString(), didDoc.AuthenticationKids[0]);
        Assert.Equivalent(expectedAuth2["id"].ToString(), didDoc.AuthenticationKids[1]);
        Assert.Equivalent(expectedAgreem["id"].ToString(), didDoc.AgreementKids[0]);
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2Service2Elements()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_2_SERVICES, didDoc.Did);

        Assert.NotNull(didDoc.Services);
        Assert.Equal(2, didDoc.Services.Count);

        var service1 = didDoc.Services[0];
        var expectedServiceArray1 = ((JsonElement)(Fixture.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"])).EnumerateArray().First();
        var expectedService1 = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedServiceArray1.GetRawText());
        var didCommService1 = (Service)service1;
        Assert.Equal(expectedService1!["id"].ToString(), didCommService1.Id);
        Assert.Equal(expectedService1["serviceEndpoint"].ToString(), didCommService1.ServiceEndpoint);
        Assert.Equal(expectedService1["type"].ToString(), didCommService1.Type);
        Assert.Equivalent(((JsonElement)expectedService1!["routingKeys"]).EnumerateArray().Select(p => p.GetString()).ToList(), didCommService1!.RoutingKeys);
        Assert.Empty(didCommService1.Accept);

        var service2 = didDoc.Services[1];
        var expectedServiceArray2 = ((JsonElement)(Fixture.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES)["service"])).EnumerateArray().Skip(1).First();
        var expectedService2 = JsonSerializer.Deserialize<Dictionary<string, object>>(expectedServiceArray2.GetRawText());
        var didCommService2 = (Service)service2;
        Assert.Equivalent(((JsonElement)expectedService2!["accept"]).EnumerateArray().Select(p => p.GetString()).ToList(), didCommService2!.Accept);
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2NoService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES, didDoc.Did);
        Assert.Null(didDoc.Services);
        Assert.Single(didDoc.Authentications);
        Assert.Single(didDoc.KeyAgreements);
    }

    [Fact]
    public void TestDidDocFromJsonNumalgo2MinimalService()
    {
        var didDoc = DidDocPeerDid.FromJson(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES);
        Assert.Equal(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES, didDoc.Did);

        Assert.Equal(2, didDoc.Authentications.Count);
        Assert.Single(didDoc.KeyAgreements);

        var service = didDoc.Services![0];
        var didCommService = (Service)service;
        Assert.Equal(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg.SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9#didcommmessaging-0",
            didCommService.Id
        );
        Assert.Equal("https://example.com/endpoint", didCommService.ServiceEndpoint.ToString());
        Assert.Equal("DIDCommMessaging", didCommService.Type.ToString());
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