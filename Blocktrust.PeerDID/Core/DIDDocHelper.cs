namespace Blocktrust.PeerDID.Core;

using System.Text.Json;
using System.Text.Json.Nodes;
using DIDDoc;
using Types;

public static class DidDocHelper
{
    private static readonly Dictionary<string, string> TypeAgreementVerTypeToField = new()
    {
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2019.Value, PublicKeyFieldValues.Base58 },
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2020.Value, PublicKeyFieldValues.Multibase },
        { VerificationMethodTypeAgreement.JsonWebKey2020.Value, PublicKeyFieldValues.Jwk },
    };

    private static readonly Dictionary<string, string> TypeAuthenticationVerTypeToField = new()
    {
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2018.Value, PublicKeyFieldValues.Base58 },
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2020.Value, PublicKeyFieldValues.Multibase },
        { VerificationMethodTypeAuthentication.JsonWebKey2020.Value, PublicKeyFieldValues.Jwk },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> TypeAgreementVerTypeToFormat = new()
    {
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2019.Value, VerificationMaterialFormatPeerDid.Base58 },
        { VerificationMethodTypeAgreement.X25519KeyAgreementKey2020.Value, VerificationMaterialFormatPeerDid.Multibase },
        { VerificationMethodTypeAgreement.JsonWebKey2020.Value, VerificationMaterialFormatPeerDid.Jwk },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> TypeAuthenticationVerTypeToFormat = new()
    {
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2018.Value, VerificationMaterialFormatPeerDid.Base58 },
        { VerificationMethodTypeAuthentication.Ed25519VerificationKey2020.Value, VerificationMaterialFormatPeerDid.Multibase },
        { VerificationMethodTypeAuthentication.JsonWebKey2020.Value, VerificationMaterialFormatPeerDid.Jwk },
    };

    public static DidDocPeerDid DidDocFromJson(JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue(DidDocConstants.Id, out JsonNode? didJsonNode);
        var did = didJsonNode.AsValue().ToString();

        if (did == null)
        {
            throw new ArgumentException("No 'id' field");
        }

        var authentication = new List<VerificationMethodPeerDid>();
        jsonObject.TryGetPropertyValue(DidDocConstants.Authentication, out JsonNode? authenticationJsonNode);
        if (authenticationJsonNode is not null)
        {
            authentication = authenticationJsonNode?.AsArray().Select(p => VerificationMethodFromJson(p.AsObject())).ToList();
        }

        jsonObject.TryGetPropertyValue(DidDocConstants.KeyAgreement, out JsonNode? keyAgreementJsonNode);
        var keyAgreement = new List<VerificationMethodPeerDid>();
        if (keyAgreementJsonNode is not null)
        {
            keyAgreement = keyAgreementJsonNode?.AsArray().Select(p => VerificationMethodFromJson(p.AsObject())).ToList();
        }

        List<PeerDidService>? service = null;
        jsonObject.TryGetPropertyValue(DidDocConstants.Service, out JsonNode? serviceJsonNode);
        if (serviceJsonNode is not null)
        {
            service = serviceJsonNode?.AsArray().Select(p => ServiceFromJson(p.AsObject())).ToList();
        }

        return new DidDocPeerDid(did, authentication, keyAgreement, service);
    }


    private static VerificationMethodPeerDid VerificationMethodFromJson(JsonObject jsonObject)
    {
        Dictionary<string, object> serviceMap = Utils.FromJsonToMap(jsonObject.ToString());
        string id = jsonObject[ServiceConstants.ServiceId]?.ToString();

        if (id == null)
        {
            throw new System.Exception("No 'id' field in method " + jsonObject.ToString());
        }

        string controller = jsonObject[DidDocConstants.Controller]?.ToString();
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
        if (verMaterialType == VerificationMethodTypeAgreement.JsonWebKey2020 ||
            verMaterialType == VerificationMethodTypeAuthentication.JsonWebKey2020)
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

    private static PeerDidService ServiceFromJson(JsonObject jsonObject)
    {
        Dictionary<string, object> serviceMap = Utils.FromJsonToMap(jsonObject.ToString());
        string id = jsonObject[ServiceConstants.ServiceId]?.ToString();
        if (id == null)
        {
            throw new System.ArgumentException("No 'id' field in service " + jsonObject.ToString());
        }

        string type = jsonObject[ServiceConstants.ServiceType]?.ToString();
        if (type == null)
        {
            throw new System.ArgumentException("No 'type' field in service " + jsonObject.ToString());
        }

        if (type != ServiceConstants.ServiceDidcommMessaging)
        {
            return PeerDidService.FromDictionary(serviceMap);
        }

        string endpoint = jsonObject[ServiceConstants.ServiceEndpoint]?.ToString();
        List<string>? routingKeys = jsonObject[ServiceConstants.ServiceRoutingKeys]?.AsArray()?.Select(x => x.ToString()).ToList();
        List<string> accept = jsonObject[ServiceConstants.ServiceAccept]?.AsArray()?.Select(x => x.ToString()).ToList();

        return new PeerDidService(
            id: id,
            type: type,
            serviceEndpoint: endpoint ?? "",
            routingKeys: routingKeys ?? new List<string>(),
            accept: accept ?? new List<string>()
        );
    }

    private static VerificationMethodTypePeerDid GetVerMethodType(JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue(DidDocConstants.Type, out JsonNode? typeJsonNode);
        var type = typeJsonNode.AsValue().ToString();

        if (type.Equals(VerificationMethodTypeAgreement.X25519KeyAgreementKey2019.Value))
        {
            return VerificationMethodTypeAgreement.X25519KeyAgreementKey2019;
        }

        else if (type.Equals(VerificationMethodTypeAgreement.X25519KeyAgreementKey2020.Value))
        {
            return VerificationMethodTypeAgreement.X25519KeyAgreementKey2020;
        }

        else if (type.Equals(VerificationMethodTypeAuthentication.Ed25519VerificationKey2018.Value))
        {
            return VerificationMethodTypeAuthentication.Ed25519VerificationKey2018;
        }

        else if (type.Equals(VerificationMethodTypeAuthentication.Ed25519VerificationKey2020.Value))
        {
            return VerificationMethodTypeAuthentication.Ed25519VerificationKey2020;
        }

        else if (type.Equals(VerificationMethodTypeAuthentication.JsonWebKey2020.Value))
        {
            jsonObject.TryGetPropertyValue(PublicKeyFieldValues.Jwk, out JsonNode? vJsonNode);
            var crv = vJsonNode!["crv"].ToString();
            if (crv == "X25519")
            {
                return VerificationMethodTypeAgreement.JsonWebKey2020;
            }
            else
            {
                return VerificationMethodTypeAuthentication.JsonWebKey2020;
            }
        }
        else
        {
            throw new System.Exception("Unknown verification method type " + type);
        }
    }
}