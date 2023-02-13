namespace Blocktrust.PeerDID.DIDDoc;

using System.Text.Json;
using System.Text.Json.Serialization;
using Types;

public class VerificationMethodPeerDIDConverter : JsonConverter<VerificationMethodPeerDid>
{
    public override void Write(Utf8JsonWriter writer, VerificationMethodPeerDid value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override VerificationMethodPeerDid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? propertyName = String.Empty;
        var verificationMethodPeerDid = new VerificationMethodPeerDid();
        string format = null;
        var type = "";
        var value = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                switch (format)
                {
                    case "publicKeyMultibase":
                        verificationMethodPeerDid.VerMaterial = new VerificationMaterialPeerDid<VerificationMethodTypePeerDid>(VerificationMaterialFormatPeerDid.MULTIBASE, value, null);
                        break;
                    case "publicKeyBase58":
                        verificationMethodPeerDid.VerMaterial = new VerificationMaterialPeerDid<VerificationMethodTypePeerDid>(VerificationMaterialFormatPeerDid.BASE58, value, null);
                        break;
                    case "publicKeyJwk":
                        verificationMethodPeerDid.VerMaterial = new VerificationMaterialPeerDid<VerificationMethodTypePeerDid>(VerificationMaterialFormatPeerDid.JWK, value, null);
                        break;
                }

                return verificationMethodPeerDid;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString()!.ToLowerInvariant();
                reader.Read();
                switch (propertyName)
                {
                    case "id":
                        verificationMethodPeerDid.Id = reader.GetString();
                        break;
                    case "controller":
                        verificationMethodPeerDid.Controller = reader.GetString();
                        break;
                    case "type":
                        type = reader.GetString();
                        break;
                    case "publickeymultibase":
                        format = PublicKeyFieldValues.MULTIBASE;
                        value = reader.GetString();
                        break;
                    case "publickeybase58":
                        format = PublicKeyFieldValues.BASE58;
                        value = reader.GetString();
                        break;
                    case "publickeyjwk":
                        format = PublicKeyFieldValues.JWK;
                        value = reader.GetString();
                        break;
                }
            }
        }

        throw new JsonException();
    }
}