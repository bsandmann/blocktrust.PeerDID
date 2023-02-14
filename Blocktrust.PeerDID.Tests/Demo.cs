using Blocktrust.PeerDID.Types;

namespace Blocktrust.PeerDID.Tests;

using DIDDoc;
using PeerDIDCreateResolve;

public class Demo
{
    [Fact]
    public void TestCreateResolvePeerDid()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement>()
        {
            new VerificationMaterialAgreement(
                type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                format: VerificationMaterialFormatPeerDid.BASE58,
                value: "DmgBSHMqaZiYqwNMEJJuxWzsGGC8jUYADrfSdBrC6L8s"
            )
        };

        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication>()
        {
            new VerificationMaterialAuthentication(
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDid.BASE58,
                value: "ByHnpUCFb1vAfh9CFZ8ZkmUZguURW8nSw889hy6rD8L7"
            )
        };

        string service = @"
            {
                ""type"": ""DIDCommMessaging"",
                ""serviceEndpoint"": ""https://example.com/endpoint1"",
                ""routingKeys"": [""did:example:somemediator#somekey1""],
                ""accept"": [""didcomm/v2"", ""didcomm/aip2;env=rfc587""]
            }
        ";

        string peerDIDAlgo0 = PeerDidCreator.CreatePeerDidNumalgo0(signingKeys[0]);
        string peerDIDAlgo2 = PeerDidCreator.CreatePeerDidNumalgo2(
            encryptionKeys, signingKeys, service
        );

        string didDocAlgo0Json = PeerDidResolver.ResolvePeerDid(new PeerDid(peerDIDAlgo0), VerificationMaterialFormatPeerDid.BASE58);
        string didDocAlgo2Json = PeerDidResolver.ResolvePeerDid(new PeerDid(peerDIDAlgo2), VerificationMaterialFormatPeerDid.BASE58);

        DidDocPeerDid didDocAlgo0 = DIDDoc.DidDocPeerDid.FromJson(didDocAlgo0Json);
        DidDocPeerDid didDocAlgo2 = DIDDoc.DidDocPeerDid.FromJson(didDocAlgo2Json);
        
        //This 'test' features no assertions, but it does show how to use the library.

    }
}