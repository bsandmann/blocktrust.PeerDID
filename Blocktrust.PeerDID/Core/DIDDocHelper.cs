namespace Blocktrust.PeerDID.Core;

using System.Text.Json;
using System.Text.Json.Nodes;
using DIDDoc;
using Tests;
using Types;

public static class DidDocHelper
{
    private static readonly Dictionary<string, string> TypeAgreementVerTypeToField = new()
    {
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019.Value, PublicKeyFieldValues.BASE58 },
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020.Value, PublicKeyFieldValues.MULTIBASE },
        { VerificationMethodTypeAgreement.JSON_WEB_KEY_2020.Value, PublicKeyFieldValues.JWK },
    };

    private static readonly Dictionary<string, string> TypeAuthenticationVerTypeToField = new()
    {
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018.Value, PublicKeyFieldValues.BASE58 },
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020.Value, PublicKeyFieldValues.MULTIBASE },
        { VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020.Value, PublicKeyFieldValues.JWK },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> TypeAgreementVerTypeToFormat = new()
    {
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019.Value, VerificationMaterialFormatPeerDid.BASE58 },
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020.Value, VerificationMaterialFormatPeerDid.MULTIBASE },
        { VerificationMethodTypeAgreement.JSON_WEB_KEY_2020.Value, VerificationMaterialFormatPeerDid.JWK },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> TypeAuthenticationVerTypeToFormat = new()
    {
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018.Value, VerificationMaterialFormatPeerDid.BASE58 },
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020.Value, VerificationMaterialFormatPeerDid.MULTIBASE },
        { VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020.Value, VerificationMaterialFormatPeerDid.JWK },
    };

    public static DidDocPeerDid DidDocFromJson(JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue("id", out JsonNode? didJsonNode);
        var did = didJsonNode.AsValue().ToString();

        if (did == null)
        {
            throw new ArgumentException("No 'id' field");
        }

        var authentication = new List<VerificationMethodPeerDid>();
        jsonObject.TryGetPropertyValue("authentication", out JsonNode? authenticationJsonNode);
        if (authenticationJsonNode is not null)
        {
            authentication = authenticationJsonNode?.AsArray().Select(p => VerificationMethodFromJson(p.AsObject())).ToList();
        }

        jsonObject.TryGetPropertyValue("keyAgreement", out JsonNode? keyAgreementJsonNode);
        var keyAgreement = new List<VerificationMethodPeerDid>();
        if (keyAgreementJsonNode is not null)
        {
            keyAgreement = keyAgreementJsonNode?.AsArray().Select(p => VerificationMethodFromJson(p.AsObject())).ToList();
        }

        List<Service>? service = null;
        jsonObject.TryGetPropertyValue("service", out JsonNode? serviceJsonNode);
        if (serviceJsonNode is not null)
        {
            service = serviceJsonNode?.AsArray().Select(p => ServiceFromJson(p.AsObject())).ToList();
        }

        return new DidDocPeerDid(did, authentication, keyAgreement, service);
    }


    private static VerificationMethodPeerDid VerificationMethodFromJson(JsonObject jsonObject)
    {
        Dictionary<string, object> serviceMap = Utils.FromJsonToMap(jsonObject.ToString());
        string id = jsonObject[ServiceConstants.SERVICE_ID]?.ToString();

        if (id == null)
        {
            throw new System.Exception("No 'id' field in method " + jsonObject.ToString());
        }

        string controller = jsonObject["controller"]?.ToString();
        if (controller == null) throw new System.Exception("No 'controller' field in method " + jsonObject.ToString());

        VerificationMethodTypePeerDid verMaterialType = GetVerMethodType(jsonObject);
        var field = String.Empty;
        if (verMaterialType is VerificationMethodTypeAgreement)
        {
            field = TypeAgreementVerTypeToField[verMaterialType.Value];
        }
        else if (verMaterialType is VerificationMethodTypeAuthentication)
        {
            field = TypeAuthenticationVerTypeToField[verMaterialType.Value];
        }
        else
        {
            throw new Exception("Unkonwn verification method type: " + verMaterialType.Value);
        }

        VerificationMaterialFormatPeerDid format;
        if (verMaterialType is VerificationMethodTypeAgreement)
        {
            format = TypeAgreementVerTypeToFormat[verMaterialType.Value];
        }
        else if (verMaterialType is VerificationMethodTypeAuthentication)
        {
            format = TypeAuthenticationVerTypeToFormat[verMaterialType.Value];
        }
        else
        {
            throw new Exception("Unkonwn verification method type: " + verMaterialType.Value);
        }

        object value = null;
        if (verMaterialType == VerificationMethodTypeAgreement.JSON_WEB_KEY_2020 ||
            verMaterialType == VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020)
        {
            var jwkJson = JsonSerializer.Deserialize<PeerDidJwk>(jsonObject[field]);
            if (jwkJson == null) throw new System.Exception("No 'field' field in method " + jsonObject.ToString());
            value = jwkJson;
        }
        else
        {
            value = jsonObject[field].GetValue<string>();
        }

        return new VerificationMethodPeerDid()
        {
            Id = id,
            Controller = controller,
            VerMaterial = new VerificationMaterialPeerDid<VerificationMethodTypePeerDid>(
                format: format,
                type: verMaterialType,
                value: value)
        };
    }

    private static Service ServiceFromJson(JsonObject jsonObject)
    {
        Dictionary<string, object> serviceMap = Utils.FromJsonToMap(jsonObject.ToString());
        string id = jsonObject[ServiceConstants.SERVICE_ID]?.ToString();
        if (id == null)
        {
            throw new System.ArgumentException("No 'id' field in service " + jsonObject.ToString());
        }

        string type = jsonObject[ServiceConstants.SERVICE_TYPE]?.ToString();
        if (type == null)
        {
            throw new System.ArgumentException("No 'type' field in service " + jsonObject.ToString());
        }

        if (type != "SERVICE_DIDCOMM_MESSAGING")
        {
            return new OtherService(serviceMap);
        }

        string endpoint = jsonObject[ServiceConstants.SERVICE_ENDPOINT]?.ToString();
        List<string> routingKeys = jsonObject[ServiceConstants.SERVICE_ROUTING_KEYS]?.AsArray()?.Select(x => x.ToString()).ToList();
        List<string> accept = jsonObject[ServiceConstants.SERVICE_ACCEPT]?.AsArray()?.Select(x => x.ToString()).ToList();

        return new DidCommServicePeerDid(
            id: id,
            type: type,
            serviceEndpoint: endpoint ?? "",
            routingKeys: routingKeys ?? new List<string>(),
            accept: accept ?? new List<string>()
        );
    }

    private static VerificationMethodTypePeerDid GetVerMethodType(JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue("type", out JsonNode? typeJsonNode);
        var type = typeJsonNode.AsValue().ToString();

        if (type.Equals(VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019.Value))
        {
            return VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019;
        }

        else if (type.Equals(VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020.Value))
        {
            return VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020;
        }

        else if (type.Equals(VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018.Value))
        {
            return VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018;
        }

        else if (type.Equals(VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020.Value))
        {
            return VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020;
        }

        else if (type.Equals(VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020.Value))
        {
            jsonObject.TryGetPropertyValue(PublicKeyFieldValues.JWK, out JsonNode? vJsonNode);
            var crv = vJsonNode!["crv"].ToString();
            if (crv == "X25519")
            {
                return VerificationMethodTypeAgreement.JSON_WEB_KEY_2020;
            }
            else
            {
                return VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020;
            }
        }
        else
        {
            throw new System.Exception("Unknown verification method type " + type);
        }
    }
}