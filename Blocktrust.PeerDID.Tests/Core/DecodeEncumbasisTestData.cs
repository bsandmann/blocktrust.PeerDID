namespace Blocktrust.PeerDID.Tests.Core;

using Types;

public class DecodeEncumbasisTestData
{
    public string InputMultibase { get; set; }
    public VerificationMaterialFormatPeerDid Format { get; set; }
    public VerificationMaterialPeerDid<VerificationMethodTypePeerDid> Expected { get; set; }

    public DecodeEncumbasisTestData(
        string inputMultibase,
        VerificationMaterialFormatPeerDid format,
        VerificationMaterialPeerDid<VerificationMethodTypePeerDid> expected)
    {
        InputMultibase = inputMultibase;
        Format = format;
        Expected = expected;
    }
}
