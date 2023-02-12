namespace Blocktrust.PeerDID.Core;

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Common.Converter;
using DIDDoc;
using Types;

public class PeerDIDHelper
{
    private static readonly Dictionary<string, string> ServicePrefix = new Dictionary<string, string>
    {
        { ServiceConstants.SERVICE_TYPE, "t" },
        { ServiceConstants.SERVICE_ENDPOINT, "s" },
        { ServiceConstants.SERVICE_DIDCOMM_MESSAGING, "dm" },
        { ServiceConstants.SERVICE_ROUTING_KEYS, "r" },
        { ServiceConstants.SERVICE_ACCEPT, "a" }
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
        ValidateJson(service);
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
    /// <param name="peerDID">PeerDID which will be used as an ID</param>
    /// <returns>decoded service</returns>
    /// <exception cref="ArgumentException">if service is not correctly decoded</exception>
    public static List<Service> DecodeService(string encodedService, PeerDID peerDID)
    {
        if (encodedService == "")
        {
            return null;
        }

        var decodedService = Encoding.UTF8.GetString(Convert.FromBase64String(encodedService));

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

        var otherServiceList = serviceMapList.Select((serviceMap, serviceNumber) =>
        {
            if (!serviceMap.ContainsKey(ServicePrefix["SERVICE_TYPE"]))
            {
                throw new ArgumentException("service doesn't contain a type");
            }

            var f = serviceMap[ServicePrefix["SERVICE_TYPE"]];
            var s = f.ToString();
            var serviceType = s.Replace(ServicePrefix["SERVICE_DIDCOMM_MESSAGING"], ServiceConstants.SERVICE_DIDCOMM_MESSAGING);
            var service = new Dictionary<string, object>
            {
                { "id", $"{peerDID}#{((string)serviceType).ToLower()}-{serviceNumber}" },
                { "type", serviceType }
            };

            if (serviceMap.ContainsKey(ServicePrefix["SERVICE_ENDPOINT"]))
            {
                service["SERVICE_ENDPOINT"] = serviceMap[ServicePrefix["SERVICE_ENDPOINT"]];
            }

            if (serviceMap.ContainsKey(ServicePrefix["SERVICE_ROUTING_KEYS"]))
            {
                service["SERVICE_ROUTING_KEYS"] = serviceMap[ServicePrefix["SERVICE_ROUTING_KEYS"]];
            }

            if (serviceMap.ContainsKey(ServicePrefix["SERVICE_ACCEPT"]))
            {
                service["SERVICE_ACCEPT"] = serviceMap[ServicePrefix["SERVICE_ACCEPT"]];
            }

            return new OtherService(service);
        }).ToList().Select(p => (Service)p).ToList();
        return otherServiceList;
    }

    /// <summary>
    /// Creates multibased encnumbasis according to PeerDID spec
    /// <a href="https://identity.foundation/peer-did-method-spec/index.html#method-specific-identifier">Specification</a>
    /// </summary>
    /// <param name="key">public key</param>
    /// <returns>transform+encnumbasis</returns>
    /// <exception cref="ArgumentOutOfRangeException">if key is invalid</exception>
    internal static string CreateMultibaseEncnumbasis(VerificationMaterialPeerDID<VerificationMethodTypePeerDID> key)
    {
        byte[] decodedKey;

        switch (key.Format)
        {
            case VerificationMaterialFormatPeerDID.BASE58:
                decodedKey = Multibase.FromBase58(key.Value.ToString());
                break;
            case VerificationMaterialFormatPeerDID.MULTIBASE:
                //TODO seconds??
                // decodedKey = Multicodec.FromMulticodec(Multibase.FromBase58Multibase(key.Value.ToString()).Second).Second;
                var x = Multibase.FromBase58Multibase(key.Value.ToString());

                decodedKey = Multicodec.FromMulticodec(x.Item2).Value;
                break;
            case VerificationMaterialFormatPeerDID.JWK:
                decodedKey = JWK_OKP.FromJwk(key);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Validation.ValidateRawKeyLength(decodedKey);
        return Multibase.ToBase58Multibase(Multicodec.ToMulticodec(decodedKey, key.Type));
    }

    /// <summary>
    /// Decodes multibased encnumbasis to a verification material for DID DOC
    /// </summary>
    /// <param name="multibase">transform+encnumbasis to decode</param>
    /// <param name="format">the format of public keys in the DID DOC</param>
    /// <exception cref="ArgumentOutOfRangeException">if key is invalid</exception>
    /// <returns>decoded encnumbasis as verification material for DID DOC</returns>
    public static DecodedEncumbasis DecodeMultibaseEncnumbasis(string multibase, VerificationMaterialFormatPeerDID format)
    {
        var (encnumbasis, decodedEncnumbasis) = Multibase.FromBase58Multibase(multibase);
        var (codec, decodedEncnumbasisWithoutPrefix) = Multicodec.FromMulticodec(decodedEncnumbasis);
        Validation.ValidateRawKeyLength(decodedEncnumbasisWithoutPrefix);

        VerificationMaterialPeerDID<VerificationMethodTypePeerDID> verMaterial = null;
        switch (format)
        {
            case VerificationMaterialFormatPeerDID.BASE58:
                switch (codec.Name)
                {
                    case Multicodec.NameX25519:
                        verMaterial = new VerificationMaterialAgreement(
                            format: format,
                            type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                            value: Multibase.ToBase58(decodedEncnumbasisWithoutPrefix)
                        );
                        break;
                    case Multicodec.NameED25519:
                        verMaterial = new VerificationMaterialAuthentication(
                            format: format,
                            type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                            value: Multibase.ToBase58(decodedEncnumbasisWithoutPrefix)
                        );
                        break;
                }

                break;
            case VerificationMaterialFormatPeerDID.MULTIBASE:
                switch (codec.Name)
                {
                    case Multicodec.NameX25519:
                        verMaterial = new VerificationMaterialAgreement(
                            format: format,
                            type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
                            value: Multibase.ToBase58Multibase(
                                Multicodec.ToMulticodec(
                                    decodedEncnumbasisWithoutPrefix,
                                    VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020
                                )
                            )
                        );
                        break;
                    case Multicodec.NameED25519:
                        verMaterial = new VerificationMaterialAuthentication(
                            format: format,
                            type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                            value: Multibase.ToBase58Multibase(
                                Multicodec.ToMulticodec(
                                    decodedEncnumbasisWithoutPrefix,
                                    VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020
                                )
                            )
                        );
                        break;
                }

                break;
            case VerificationMaterialFormatPeerDID.JWK:
                switch (codec.Name)
                {
                    case Multicodec.NameX25519:
                        verMaterial = new VerificationMaterialAgreement(
                            format: format,
                            type: VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
                            value: JWK_OKP.ToJwk(decodedEncnumbasisWithoutPrefix, VerificationMethodTypeAgreement.JSON_WEB_KEY_2020)
                        );
                        break;
                    case Multicodec.NameED25519:
                        verMaterial = new VerificationMaterialAuthentication(
                            format: format,
                            type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                            value: JWK_OKP.ToJwk(
                                decodedEncnumbasisWithoutPrefix,
                                VerificationMethodTypeAgreement.JSON_WEB_KEY_2020)
                        );
                        break;
                }

                break;
        }

        return new DecodedEncumbasis(encnumbasis, verMaterial);
    }

    internal static VerificationMethodPeerDID GetVerificationMethod(string did, DecodedEncumbasis decodedEncumbasis)
    {
        return new VerificationMethodPeerDID
        {
            Id = $"{did}#{decodedEncumbasis.Encnumbasis}",
            Controller = did,
            VerMaterial = decodedEncumbasis.VerMaterial
        };
    }
    
    private static void ValidateJson(string service)
    {
        try
        {
            JsonDocument.Parse(service);
        }
        catch (JsonException e)
        {
            throw new ArgumentException("Invalid JSON");
        }
    }
}