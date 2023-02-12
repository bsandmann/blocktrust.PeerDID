namespace Blocktrust.PeerDID.Tests;

using Types;

public class TestData
{
    public List<VerificationMaterialAuthentication> signingKeys { get; set; }
    public List<VerificationMaterialAgreement> encKeys { get; set; }
    
    public TestData(List<VerificationMaterialAuthentication> signingKeys, List<VerificationMaterialAgreement> encKeys)
    {
        this.signingKeys = signingKeys;
        this.encKeys = encKeys;
    }

    public TestData()
    {
    }
}