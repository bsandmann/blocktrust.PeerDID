namespace Blocktrust.PeerDID.Core;

using System.Text.Json.Nodes;
using DIDDoc;
using Types;

public static class DidDocHelper
{
    private static readonly Dictionary<string, string> VerTypeToField = new Dictionary<string, string>()
    {
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019.Value, PublicKeyFieldValues.BASE58 },
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020.Value, PublicKeyFieldValues.MULTIBASE },
        { VerificationMethodTypeAgreement.JSON_WEB_KEY_2020.Value, PublicKeyFieldValues.JWK },
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018.Value, PublicKeyFieldValues.BASE58 },
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020.Value, PublicKeyFieldValues.MULTIBASE },
        { VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020.Value, PublicKeyFieldValues.JWK },
    };

    private static readonly Dictionary<string, VerificationMaterialFormatPeerDid> VerTypeToFormat = new Dictionary<string, VerificationMaterialFormatPeerDid>()
    {
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019.Value, VerificationMaterialFormatPeerDid.BASE58 },
        { VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020.Value, VerificationMaterialFormatPeerDid.MULTIBASE },
        { VerificationMethodTypeAgreement.JSON_WEB_KEY_2020.Value, VerificationMaterialFormatPeerDid.JWK },
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018.Value, VerificationMaterialFormatPeerDid.BASE58 },
        { VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020.Value, VerificationMaterialFormatPeerDid.MULTIBASE },
        { VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020.Value, VerificationMaterialFormatPeerDid.JWK },
    };

    internal static DidDocPeerDid DidDocFromJson(JsonObject jsonObject)
    {
        //TODO
        jsonObject.TryGetPropertyValue("id", out JsonNode? didJsonNode);
        var did = didJsonNode.AsValue().ToString(); 
        // string did = jsonObject.Get("id")?.AsString;
        
        if (did == null)
        {
            throw new ArgumentException("No 'id' field");
        }

        //TODO
        jsonObject.TryGetPropertyValue("authentication", out JsonNode? authenticationJsonNode);
        List<VerificationMethodPeerDid> authentication = authenticationJsonNode.AsArray().Select(p => VerificationMethodFromJson(p.AsObject())).ToList();
        // IList<VerificationMethodPeerDID> authentication = jsonObject.Get("authentication")
        //                                                       ?.AsJsonArray
        //                                                       ?.Select(item => VerificationMethodFromJson(item.AsJsonObject))
        //                                                       .ToList()
        //                                                   ?? new List<VerificationMethodPeerDID>();
        
        //TODO
        jsonObject.TryGetPropertyValue("keyAgreement", out JsonNode? keyAgreementJsonNode);
        var keyAgreement = keyAgreementJsonNode.AsArray().Select(p => VerificationMethodFromJson(p.AsObject())).ToList();
        // IList<VerificationMethodPeerDID> keyAgreement = jsonObject.Get("keyAgreement")
        //                                                     ?.AsJsonArray
        //                                                     ?.Select(item => VerificationMethodFromJson(item.AsJsonObject))
        //                                                     .ToList()
        //                                                 ?? new List<VerificationMethodPeerDID>();
        
        //TODO
        jsonObject.TryGetPropertyValue("service", out JsonNode? serviceJsonNode);
        var service = serviceJsonNode.AsArray().Select(p => ServiceFromJson(p.AsObject())).ToList();
        
        // IList<Service> service = jsonObject.Get("service")
        //     ?.AsJsonArray
        //     ?.Select(item => serviceFromJson(item.AsJsonObject))
        //     .ToList();
        return new DidDocPeerDid(did, authentication, keyAgreement, service);
    }


    internal static VerificationMethodPeerDid VerificationMethodFromJson(JsonObject jsonObject)
    {
        //TODO
        Dictionary<string, object> serviceMap = Utils.FromJsonToMap(jsonObject.ToString());
        string id = jsonObject["SERVICE_ID"]?.ToString(); 
        // string id = jsonObject.GetValue("id")?.ToString();
        
        if (id == null) throw new System.Exception("No 'id' field in method " + jsonObject.ToString());
        //TODO
        string controller = jsonObject["controller"]?.ToString();
        // string controller = jsonObject.GetValue("controller")?.ToString();
        if (controller == null) throw new System.Exception("No 'controller' field in method " + jsonObject.ToString());

        VerificationMethodTypePeerDid verMaterialType = GetVerMethodType(jsonObject);
        var field = VerTypeToField[verMaterialType.Value];
        VerificationMaterialFormatPeerDid format = VerTypeToFormat[verMaterialType.Value];

        object value = null;
        if (verMaterialType == VerificationMethodTypeAgreement.JSON_WEB_KEY_2020 ||
            verMaterialType == VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020)
        {
           //TODO 
            // string jwkJson = jsonObject.GetValue(field)?.ToString();
            string jwkJson = jsonObject[field].GetValue<string>();
            if (jwkJson == null) throw new System.Exception("No 'field' field in method " + jsonObject.ToString());
            value = Utils.FromJsonToMap(jwkJson);
        }
        else
        {
           //TODO 
            // string stringValue = jsonObject.GetValue(field.Value)?.ToString();
            string stringValue = jsonObject[field].GetValue<string>();
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

    internal static Service ServiceFromJson(JsonObject jsonObject)
    {
        Dictionary<string, object> serviceMap = Utils.FromJsonToMap(jsonObject.ToString());
        string id = jsonObject["SERVICE_ID"]?.ToString();
        if (id == null)
        {
            throw new System.ArgumentException("No 'id' field in service " + jsonObject.ToString());
        }

        string type = jsonObject["SERVICE_TYPE"]?.ToString();
        if (type == null)
        {
            throw new System.ArgumentException("No 'type' field in service " + jsonObject.ToString());
        }

        if (type != "SERVICE_DIDCOMM_MESSAGING")
        {
            return new OtherService(serviceMap);
        }

        string endpoint = jsonObject["SERVICE_ENDPOINT"]?.ToString();
        List<string> routingKeys = jsonObject["SERVICE_ROUTING_KEYS"]?.AsArray()?.Select(x => x.ToString()).ToList();
        List<string> accept = jsonObject["SERVICE_ACCEPT"]?.AsArray()?.Select(x => x.ToString()).ToList();

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
        //TODO
        jsonObject.TryGetPropertyValue("type", out JsonNode? typeJsonNode);
        var type = typeJsonNode.AsValue().ToString();
        // string type = jsonObject.Get("type")?.AsString
        //               ?? throw new System.Exception("No 'type' field in method " + jsonObject.ToString());


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
            //TODO
            jsonObject.TryGetPropertyValue(PublicKeyFieldValues.JWK, out JsonNode? vJsonNode);
            var v = vJsonNode.AsValue();
            var crv = v["crv"].ToString();

            // JsonObject v = jsonObject.Get(PublicKeyFieldValues.JWK)?.AsJsonObject
            //                ?? throw new System.Exception("No 'field' field in method " + jsonObject.ToString());
            // string crv = v.Get("crv")?.AsString
            //              ?? throw new System.Exception("No 'crv' field in method " + jsonObject.ToString());

            if (crv == "X25519")
                return VerificationMethodTypeAgreement.JSON_WEB_KEY_2020;
            else
                return VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020;
        }
        else
        {
            throw new System.Exception("Unknown verification method type " + type);
        }
    }
}