using System.Text.Json;
using System.Text.Json.Serialization;
using Blocktrust.Common.Models.DidDoc;

public class ServiceEndpointConverter : JsonConverter<ServiceEndpoint>
{
    public override ServiceEndpoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // JSON provides a URI string for `serviceEndpoint`
            return new ServiceEndpoint(reader.GetString());
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            // JSON provides a complex object for `serviceEndpoint`
            return JsonSerializer.Deserialize<ServiceEndpoint>(ref reader, options);
        }

        throw new JsonException("Invalid JSON for ServiceEndpoint.");
    }

    public override void Write(Utf8JsonWriter writer, ServiceEndpoint value, JsonSerializerOptions options)
    {
        if (value.RoutingKeys == null && value.Accept == null)
        {
            // Write as a simple string if only URI is set
            writer.WriteStringValue(value.Uri);
        }
        else
        {
            // Write as an object if more properties are present
            writer.WriteStartObject();
            writer.WriteString("uri", value.Uri);
            writer.WritePropertyName("routingKeys");
            JsonSerializer.Serialize(writer, value.RoutingKeys ?? new List<string>(), options);
            writer.WritePropertyName("accept");
            JsonSerializer.Serialize(writer, value.Accept ?? new List<string>(), options);
            writer.WriteEndObject();
        }
    }
}