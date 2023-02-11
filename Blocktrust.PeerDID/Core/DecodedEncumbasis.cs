namespace Blocktrust.PeerDID.Core;

using Types;

public class DecodedEncumbasis
{
    public string Encnumbasis { get; set; }
    public VerificationMaterialPeerDID<VerificationMethodTypePeerDID> VerMaterial { get; set; } 
    
    public DecodedEncumbasis(string encnumbasis, VerificationMaterialPeerDID<VerificationMethodTypePeerDID> verMaterial)
    {
        Encnumbasis = encnumbasis;
        VerMaterial = verMaterial;
    }

}