namespace Blocktrust.PeerDID.Tests;

using Types;

public class TestData
{
    public List<VerificationMaterialAuthentication> SigningKeys { get; set; }
    public List<VerificationMaterialAgreement> EncKeys { get; set; }
    
    public TestData(List<VerificationMaterialAuthentication> signingKeys, List<VerificationMaterialAgreement> encKeys)
    {
        this.SigningKeys = signingKeys;
        this.EncKeys = encKeys;
    }

    public TestData()
    {
    }
}