namespace Blocktrust.PeerDID.Tests.Core;

using PeerDID.Core;
using Types;

public class EncumbasisEncodeDecode
{
    public static IEnumerable<object[]> DecodeEncumbasisData()
    {
        var list = new List<DecodeEncumbasisTestData>
        {
            new DecodeEncumbasisTestData(
                inputMultibase: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                format: VerificationMaterialFormatPeerDid.BASE58,
                expected: new VerificationMaterialAuthentication
                (
                    format: VerificationMaterialFormatPeerDid.BASE58,
                    type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                    value: "ByHnpUCFb1vAfh9CFZ8ZkmUZguURW8nSw889hy6rD8L7"
                )),
            new DecodeEncumbasisTestData(
                inputMultibase: "z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc",
                format: VerificationMaterialFormatPeerDid.BASE58,
                expected: new VerificationMaterialAgreement
                (
                    format: VerificationMaterialFormatPeerDid.BASE58,
                    type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2019,
                    value: "JhNWeSVLMYccCk7iopQW4guaSJTojqpMEELgSLhKwRr"
                )),
            new DecodeEncumbasisTestData
            (
                inputMultibase: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                format: VerificationMaterialFormatPeerDid.MULTIBASE,
                expected: new VerificationMaterialAuthentication
                (
                    format: VerificationMaterialFormatPeerDid.MULTIBASE,
                    type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                    value: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"
                )
            ),
            new DecodeEncumbasisTestData
            (
                inputMultibase: "z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc",
                format: VerificationMaterialFormatPeerDid.MULTIBASE,
                expected: new VerificationMaterialAgreement
                (
                    format: VerificationMaterialFormatPeerDid.MULTIBASE,
                    type: VerificationMethodTypeAgreement.X25519_KEY_AGREEMENT_KEY_2020,
                    value: "z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc"
                )
            ),
            new DecodeEncumbasisTestData
            (
                inputMultibase: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                format: VerificationMaterialFormatPeerDid.JWK,
                expected: new VerificationMaterialAuthentication(
                    format: VerificationMaterialFormatPeerDid.JWK,
                    type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                    value: new Dictionary<string, object>
                    {
                        { "kty", "OKP" },
                        { "crv", "Ed25519" },
                        { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_YgmA" }, 
                    })
            ),
            new DecodeEncumbasisTestData
            (
                inputMultibase: "z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc",
                format: VerificationMaterialFormatPeerDid.JWK,
                expected: new VerificationMaterialAgreement
                (
                    format: VerificationMaterialFormatPeerDid.JWK,
                    type: VerificationMethodTypeAgreement.JSON_WEB_KEY_2020,
                    value: new Dictionary<string, object>
                    {
                        { "kty", "OKP" },
                        { "crv", "X25519" },
                        { "x", "BIiFcQEn3dfvB2pjlhOQQour6jXy9d5s2FKEJNTOJik" },
                    }
                )
            ),
        };
        return list.Select(p => new object[] { p });
    }

    [Theory]
    [MemberData(nameof(DecodeEncumbasisData))]
    public void TestDecodeEncumbasis(DecodeEncumbasisTestData data)
    {
        Assert.Equivalent(data.Expected, PeerDidHelper.DecodeMultibaseEncnumbasis(data.InputMultibase, data.Format).VerMaterial);
    }
}