namespace Blocktrust.Common.Models.Secrets;

using System.Text.Json.Serialization;
using Blocktrust.Common.Models.DidDoc;

public class Secret
{
    public string Kid { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationMethodType Type { get; set; }
    public VerificationMaterial VerificationMaterial { get; set; }

    public Secret()
    {
        
    }
    
    public Secret(string kid, VerificationMethodType type, VerificationMaterial verificationMaterial)
    {
        this.Kid = kid;
        this.Type = type;
        this.VerificationMaterial = verificationMaterial;
    }
}