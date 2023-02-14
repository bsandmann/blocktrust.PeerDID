namespace Blocktrust.PeerDID.Core;

using Types;

public class DecodedEncumbasis
{
    public string Encnumbasis { get; }
    public VerificationMaterialPeerDid<VerificationMethodTypePeerDid> VerMaterial { get; } 
    
    public DecodedEncumbasis(string encnumbasis, VerificationMaterialPeerDid<VerificationMethodTypePeerDid> verMaterial)
    {
        Encnumbasis = encnumbasis;
        VerMaterial = verMaterial;
    }

}