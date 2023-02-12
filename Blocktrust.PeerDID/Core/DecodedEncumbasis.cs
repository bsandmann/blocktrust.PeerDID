namespace Blocktrust.PeerDID.Core;

using Types;

public class DecodedEncumbasis
{
    public string Encnumbasis { get; set; }
    public VerificationMaterialPeerDid<VerificationMethodTypePeerDid> VerMaterial { get; set; } 
    
    public DecodedEncumbasis(string encnumbasis, VerificationMaterialPeerDid<VerificationMethodTypePeerDid> verMaterial)
    {
        Encnumbasis = encnumbasis;
        VerMaterial = verMaterial;
    }

}