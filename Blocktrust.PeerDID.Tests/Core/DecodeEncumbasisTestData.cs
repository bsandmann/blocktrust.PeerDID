namespace Blocktrust.PeerDID.Tests.Core;

using Types;

public class DecodeEncumbasisTestData
{
    public string InputMultibase { get; set; }
    public VerificationMaterialFormatPeerDID Format { get; set; }
    public VerificationMaterialPeerDID<VerificationMethodTypePeerDID> Expected { get; set; }

    public DecodeEncumbasisTestData(
        string inputMultibase,
        VerificationMaterialFormatPeerDID format,
        VerificationMaterialPeerDID<VerificationMethodTypePeerDID> expected)
    {
        InputMultibase = inputMultibase;
        Format = format;
        Expected = expected;
    }
}
