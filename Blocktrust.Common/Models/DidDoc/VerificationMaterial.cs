namespace Blocktrust.Common.Models.DidDoc;

using System.Text.Json.Serialization;

public struct VerificationMaterial
{
    public VerificationMaterial(VerificationMaterialFormat format, string value)
    {
        this.Format = format;
        this.Value = value;
    }

    
    // For serialization
    public VerificationMaterial()
    {
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public VerificationMaterialFormat Format { get; set; }
    public string Value { get; set; }
}