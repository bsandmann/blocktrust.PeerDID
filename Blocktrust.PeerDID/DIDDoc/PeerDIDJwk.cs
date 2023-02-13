namespace Blocktrust.PeerDID.Tests;

using System.Text.Json.Serialization;

public class PeerDidJwk
{
    [JsonPropertyName("kty")] public string Kty { get; set; }
    [JsonPropertyName("crv")] public string Crv { get; set; }
    [JsonPropertyName("x")] public string X { get; set; }
}
