using Blocktrust.PeerDID.Types;

namespace Blocktrust.PeerDID.Tests;

using DIDDoc;
using PeerDIDCreateResolve;
using PeerDID = Types.PeerDID;

class Demo
{
    public void TestCreateResolvePeerDID()
    {
        List<VerificationMaterialAgreement> encryptionKeys = new List<VerificationMaterialAgreement>()
        {
            new VerificationMaterialAgreement(
                type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                format: VerificationMaterialFormatPeerDID.BASE58,
                value: "DmgBSHMqaZiYqwNMEJJuxWzsGGC8jUYADrfSdBrC6L8s"
            )
        };

        List<VerificationMaterialAuthentication> signingKeys = new List<VerificationMaterialAuthentication>()
        {
            new VerificationMaterialAuthentication(
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDID.BASE58,
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

        string peerDIDAlgo0 = PeerDIDCreator.CreatePeerDIDNumalgo0(signingKeys[0]);
        string peerDIDAlgo2 = PeerDIDCreator.CreatePeerDIDNumalgo2(
            encryptionKeys, signingKeys, service
        );

        Console.WriteLine("PeerDID algo 0:" + peerDIDAlgo0);
        Console.WriteLine("==================================");
        Console.WriteLine("PeerDID algo 2:" + peerDIDAlgo2);
        Console.WriteLine("==================================");

        string didDocAlgo0Json = PeerDIDResolver.ResolvePeerDID(new PeerDID(peerDIDAlgo0));
        string didDocAlgo2Json = PeerDIDResolver.ResolvePeerDID(new PeerDID(peerDIDAlgo2));
        Console.WriteLine("DIDDoc algo 0:" + didDocAlgo0Json);
        Console.WriteLine("==================================");
        Console.WriteLine("DIDDoc algo 2:" + didDocAlgo2Json);

        DIDDocPeerDID didDocAlgo0 = DIDDoc.DIDDocPeerDID.fromJson(didDocAlgo0Json);
        DIDDocPeerDID didDocAlgo2 = DIDDoc.DIDDocPeerDID.fromJson(didDocAlgo2Json);
        Console.WriteLine("DIDDoc algo 0:" + didDocAlgo0.toDict());
        Console.WriteLine("==================================");
        Console.WriteLine("DIDDoc algo 2:" + didDocAlgo2.toDict());
    }
}