namespace Blocktrust.Common.Models.DidDoc;

using System.Text.Json.Serialization;
using Exceptions;

public class DidDoc
{
    // See DidDoc for PRISM: https://hackmd.io/-i6On7_LTRamKhEzYA6Hcw?view
    // General DidDoc spec: https://w3c.github.io/did-core/
    // Peer DidDoc spec: https://identity.foundation/peer-did-method-spec/#diddocs
    
    [JsonPropertyName(DidDocConstants.Id)]
    public string Did { get; set; }
    
    
    // In PRISM it is called "authenticationMethod"
    [JsonPropertyName(DidDocConstants.Authentication)]
    public List<string> Authentications { get; set; }
    
    /// <summary>
    /// OPTIONAL: The keyAgreement verification relationship is used to specify how an entity can generate encryption
    /// material in order to transmit confidential information intended for the DID subject, such as for
    /// the purposes of establishing a secure communication channel with the recipient.
    /// See: https://w3c.github.io/did-core/#key-agreement
    /// </summary>
    [JsonPropertyName(DidDocConstants.KeyAgreement)] 
    public List<string> KeyAgreements { get; set; }
    
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-material
    /// </summary>
    public List<VerificationMethod> VerificationMethods { get; set; }
    
    [JsonPropertyName(DidDocConstants.Service)] 
    public List<Service> Services { get; set; }

    public VerificationMethod FindVerificationMethod(string id)
    {
        var verificationMethod = VerificationMethods.Find(vm => vm.Id == id);
        if (verificationMethod == null)
        {
            throw new DidUrlNotFoundException(id, Did);
        }
    
        return verificationMethod;
    }
    
    public Service FindDidCommService(string id)
    {
        var didCommService = Services.Find(dc => dc.Id == id);
        if (didCommService == null)
        {
            throw new DidDocException($"DIDComm service '{id}' not found in DID Doc '{Did}'");
        }
    
        return didCommService;
    }
}