namespace Blocktrust.Common.Models.DidDoc;

public class VerificationMethod {
    /// <summary>
    /// Required: Id. String that is a DID
    /// </summary>
    public string Id;
   
    /// <summary>
    /// The value of the type property MUST be a string that references exactly one verification method type
    /// </summary>
    public VerificationMethodType Type;
    public VerificationMaterial VerificationMaterial;
    
    /// <summary>
    /// Also a string that is a DID
    /// </summary>
    public string Controller;

    public VerificationMethod(string id, VerificationMethodType type, VerificationMaterial verificationMaterial, string controller) {
        this.Id = id;
        this.Type = type;
        this.VerificationMaterial = verificationMaterial;
        this.Controller = controller;
    }
}
