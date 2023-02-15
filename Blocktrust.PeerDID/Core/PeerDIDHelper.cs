namespace Blocktrust.PeerDID.Core;

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Common.Converter;
using DIDDoc;
using Types;

public class PeerDidHelper
{
    private static readonly Dictionary<string, string> ServicePrefix = new()
    {
        { ServiceConstants.ServiceType, "t" },
        { ServiceConstants.ServiceEndpoint, "s" },
        { ServiceConstants.ServiceDidcommMessaging, "dm" },
        { ServiceConstants.ServiceRoutingKeys, "r" },
        { ServiceConstants.ServiceAccept, "a" }
    };

    /// <summary>
    /// Encodes [service] according to the second algorithm.
    /// For this type of algorithm DIDDoc can be obtained from PeerDID
    /// <a href="https://identity.foundation/peer-did-method-spec/index.html#generation-method">Specification</a>
    /// </summary>
    /// <param name="service">service to encode</param>
    /// <returns>encoded service</returns>
    public static string EncodeService(string service)
    {
        Validation.ValidateJson(service);
        var serviceToEncode = Regex.Replace(service, "[\n\t\\s]*", "");
        foreach (var entry in ServicePrefix)
        {
            serviceToEncode = serviceToEncode.Replace(entry.Key, entry.Value);
        }

        var encodedService = Base64Url.Encode(Encoding.UTF8.GetBytes(serviceToEncode));
        encodedService = encodedService.Replace("/", "_").Replace("+", "-");
        return "." + Numalgo2Prefix.SERVICE + encodedService;
    }


    /// <summary>
    /// Decodes [encodedService] according to PeerDID spec
    ///<a href="https://identity.foundation/peer-did-method-spec/index.html#example-2-abnf-for-peer-dids">Specification</a>
    /// </summary>
    /// <param name="encodedService">service to decod</param>
    /// <param name="peerDid">PeerDID which will be used as an ID</param>
    /// <returns>decoded service</returns>
    /// <exception cref="ArgumentException">if service is not correctly decoded</exception>
    public static List<PeerDidService> DecodeService(string encodedService, PeerDid peerDid)
    {
        if (encodedService == "")
        {
            return null;
        }

        var decodedService = Base64Url.Decode(encodedService);

        List<Dictionary<string, object>> serviceMapList;
        try
        {
            serviceMapList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(decodedService);
        }
        catch (JsonException ex)
        {
            try
            {
                serviceMapList = new List<Dictionary<string, object>>
                {
                    JsonSerializer.Deserialize<Dictionary<string, object>>(decodedService)
                };
            }
            catch (JsonException e)
            {
                throw new ArgumentException("Invalid JSON " + decodedService);
            }
        }

        var otherServiceList2 = new List<PeerDidService>();
        for (int i = 0; i < serviceMapList.Count; i++)
        {
            var serviceMap = serviceMapList[i];
            if (!serviceMap.ContainsKey(ServicePrefix[ServiceConstants.ServiceType]))
            {
                throw new ArgumentException("service doesn't contain a type");
            }

            var f = serviceMap[ServicePrefix[ServiceConstants.ServiceType]];
            var s = f.ToString();
            var serviceType = s.Replace(ServicePrefix[ServiceConstants.ServiceDidcommMessaging], ServiceConstants.ServiceDidcommMessaging);
            var service = new Dictionary<string, object>
            {
                { ServiceConstants.ServiceId, $"{peerDid.Value}#{((string)serviceType).ToLower()}-{i}" },
                { ServiceConstants.ServiceType, serviceType }
            };

            if (serviceMap.ContainsKey(ServicePrefix[ServiceConstants.ServiceEndpoint]))
            {
                var obj = serviceMap[ServicePrefix[ServiceConstants.ServiceEndpoint]];
                if (obj is JsonElement)
                {
                    var jsonElement = (JsonElement)obj;
                    if (jsonElement.ValueKind == JsonValueKind.String)
                    {
                        service[ServiceConstants.ServiceEndpoint] = jsonElement.GetString() ?? string.Empty;
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected value kind in JSON element");
                    }
                }
                else
                {
                    service[ServiceConstants.ServiceEndpoint] = obj;
                }
            }

            if (serviceMap.ContainsKey(ServicePrefix[ServiceConstants.ServiceRoutingKeys]))
            {
                var obj = serviceMap[ServicePrefix[ServiceConstants.ServiceRoutingKeys]];
                if (obj is JsonElement)
                {
                    var jsonElement = (JsonElement)obj;
                    if (jsonElement.ValueKind == JsonValueKind.String)
                    {
                        service[ServiceConstants.ServiceRoutingKeys] = jsonElement.GetString() ?? string.Empty;
                    }
                    else if (jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        service[ServiceConstants.ServiceRoutingKeys] = jsonElement.EnumerateArray().Select(p => p.GetString()).ToList();
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected value kind in JSON element");
                    }
                }
                else
                {
                    service[ServiceConstants.ServiceRoutingKeys] = obj;
                }
            }

            if (serviceMap.ContainsKey(ServicePrefix[ServiceConstants.ServiceAccept]))
            {
                var obj = serviceMap[ServicePrefix[ServiceConstants.ServiceAccept]];
                if (obj is JsonElement)
                {
                    var jsonElement = (JsonElement)obj;
                    if (jsonElement.ValueKind == JsonValueKind.String)
                    {
                        service[ServiceConstants.ServiceAccept] = jsonElement.GetString() ?? string.Empty;
                    }
                    else if (jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        service[ServiceConstants.ServiceAccept] = jsonElement.EnumerateArray().Select(p => p.GetString()).ToList();
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected value kind in JSON element");
                    }
                }
                else
                {
                    service[ServiceConstants.ServiceAccept] = obj;
                }
            }

            otherServiceList2.Add(PeerDidService.FromDictionary(service));
        }

        return otherServiceList2;
    }

    /// <summary>
    /// Creates multibased encnumbasis according to PeerDID spec
    /// <a href="https://identity.foundation/peer-did-method-spec/index.html#method-specific-identifier">Specification</a>
    /// </summary>
    /// <param name="key">public key</param>
    /// <returns>transform+encnumbasis</returns>
    /// <exception cref="ArgumentOutOfRangeException">if key is invalid</exception>
    public static string CreateMultibaseEncnumbasis(VerificationMaterialPeerDid<VerificationMethodTypePeerDid> key)
    {
        byte[] decodedKey;

        switch (key.Format)
        {
            case VerificationMaterialFormatPeerDid.Base58:
                decodedKey = Multibase.FromBase58(key.Value.ToString());
                break;
            case VerificationMaterialFormatPeerDid.Multibase:
                var multibase = Multibase.FromBase58Multibase(key.Value.ToString());
                decodedKey = Multicodec.FromMulticodec(multibase.Item2).Value;
                break;
            case VerificationMaterialFormatPeerDid.Jwk:
                decodedKey = JwkOkp.FromJwk(key);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Validation.ValidateRawKeyLength(decodedKey);
        return Multibase.ToBase58Multibase(Multicodec.ToMulticodec(decodedKey, MulticodexExtension.GetCodec(key.Type).PrefixInt));
    }

    /// <summary>
    /// Decodes multibased encnumbasis to a verification material for DID DOC
    /// </summary>
    /// <param name="multibase">transform+encnumbasis to decode</param>
    /// <param name="format">the format of public keys in the DID DOC</param>
    /// <exception cref="ArgumentOutOfRangeException">if key is invalid</exception>
    /// <returns>decoded encnumbasis as verification material for DID DOC</returns>
    public static DecodedEncumbasis DecodeMultibaseEncnumbasis(string multibase, VerificationMaterialFormatPeerDid format)
    {
        var (encnumbasis, decodedEncnumbasis) = Multibase.FromBase58Multibase(multibase);
        var (codec, decodedEncnumbasisWithoutPrefix) = Multicodec.FromMulticodec(decodedEncnumbasis);
        Validation.ValidateRawKeyLength(decodedEncnumbasisWithoutPrefix);

        VerificationMaterialPeerDid<VerificationMethodTypePeerDid> verMaterial = null;
        switch (format)
        {
            case VerificationMaterialFormatPeerDid.Base58:
                switch (codec.Name)
                {
                    case Multicodec.NameX25519:
                        verMaterial = new VerificationMaterialAgreement(
                            format: format,
                            type: VerificationMethodTypeAgreement.X25519KeyAgreementKey2019,
                            value: Multibase.ToBase58(decodedEncnumbasisWithoutPrefix)
                        );
                        break;
                    case Multicodec.NameED25519:
                        verMaterial = new VerificationMaterialAuthentication(
                            format: format,
                            type: VerificationMethodTypeAuthentication.Ed25519VerificationKey2018,
                            value: Multibase.ToBase58(decodedEncnumbasisWithoutPrefix)
                        );
                        break;
                }

                break;
            case VerificationMaterialFormatPeerDid.Multibase:
                switch (codec.Name)
                {
                    case Multicodec.NameX25519:
                        verMaterial = new VerificationMaterialAgreement(
                            format: format,
                            type: VerificationMethodTypeAgreement.X25519KeyAgreementKey2020,
                            value: Multibase.ToBase58Multibase(
                                Multicodec.ToMulticodec(
                                    decodedEncnumbasisWithoutPrefix,
                                    MulticodexExtension.GetCodec(VerificationMethodTypeAgreement.X25519KeyAgreementKey2020).PrefixInt
                                )
                            )
                        );
                        break;
                    case Multicodec.NameED25519:
                        verMaterial = new VerificationMaterialAuthentication(
                            format: format,
                            type: VerificationMethodTypeAuthentication.Ed25519VerificationKey2020,
                            value: Multibase.ToBase58Multibase(
                                Multicodec.ToMulticodec(
                                    decodedEncnumbasisWithoutPrefix,
                                    MulticodexExtension.GetCodec(VerificationMethodTypeAuthentication.Ed25519VerificationKey2020).PrefixInt
                                )
                            )
                        );
                        break;
                }

                break;
            case VerificationMaterialFormatPeerDid.Jwk:
                switch (codec.Name)
                {
                    case Multicodec.NameX25519:
                        verMaterial = new VerificationMaterialAgreement(
                            format: format,
                            type: VerificationMethodTypeAgreement.JsonWebKey2020,
                            value: JwkOkp.ToJwk(decodedEncnumbasisWithoutPrefix, VerificationMethodTypeAgreement.JsonWebKey2020)
                        );
                        break;
                    case Multicodec.NameED25519:
                        verMaterial = new VerificationMaterialAuthentication(
                            format: format,
                            type: VerificationMethodTypeAuthentication.JsonWebKey2020,
                            value: JwkOkp.ToJwk(
                                decodedEncnumbasisWithoutPrefix,
                                VerificationMethodTypeAuthentication.JsonWebKey2020)
                        );
                        break;
                }

                break;
        }

        return new DecodedEncumbasis(encnumbasis, verMaterial);
    }

    public static VerificationMethodPeerDid GetVerificationMethod(string did, DecodedEncumbasis decodedEncumbasis)
    {
        return new VerificationMethodPeerDid
        {
            Id = $"{did}#{decodedEncumbasis.Encnumbasis}",
            Controller = did,
            VerMaterial = decodedEncumbasis.VerMaterial
        };
    }
}