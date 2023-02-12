namespace Blocktrust.PeerDID.Tests;

using DIDDoc;
using Types;

public class DIDDocTestData
{
    public JSON DidDoc { get; set; }
    public VerificationMaterialFormatPeerDID ExpectedFormat { get; set; }
    public VerificationMethodTypePeerDID ExpectedAuthType { get; set; }
    public VerificationMethodTypePeerDID ExpectedAgreemType { get; set; }
    public string ExpectedField { get; set; }

    public DIDDocTestData(JSON didDoc, VerificationMaterialFormatPeerDID expectedFormat, VerificationMethodTypePeerDID expectedAuthType, VerificationMethodTypePeerDID expectedAgreemType, string expectedField)
    {
        DidDoc = didDoc;
        ExpectedFormat = expectedFormat;
        ExpectedAuthType = expectedAuthType;
        ExpectedAgreemType = expectedAgreemType;
        ExpectedField = expectedField;
    } 
}