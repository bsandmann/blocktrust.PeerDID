namespace Blocktrust.PeerDID.Tests;

using DIDDoc;
using Types;

public class DidDocTestData
{
    public Json DidDoc { get; set; }
    public VerificationMaterialFormatPeerDid ExpectedFormat { get; set; }
    public VerificationMethodTypePeerDid ExpectedAuthType { get; set; }
    public VerificationMethodTypePeerDid ExpectedAgreemType { get; set; }
    public string ExpectedField { get; set; }

    public DidDocTestData(Json didDoc, VerificationMaterialFormatPeerDid expectedFormat, VerificationMethodTypePeerDid expectedAuthType, VerificationMethodTypePeerDid expectedAgreemType, string expectedField)
    {
        DidDoc = didDoc;
        ExpectedFormat = expectedFormat;
        ExpectedAuthType = expectedAuthType;
        ExpectedAgreemType = expectedAgreemType;
        ExpectedField = expectedField;
    } 
}