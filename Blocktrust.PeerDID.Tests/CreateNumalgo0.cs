namespace Blocktrust.PeerDID.Tests;

using System.Text.Json;
using System.Text.RegularExpressions;
using PeerDID.Core;
using PeerDIDCreateResolve;
using Types;

public class CreateNumalgo0
{
    public static IEnumerable<object[]> ValidKeys()
    {
        var list = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "ByHnpUCFb1vAfh9CFZ8ZkmUZguURW8nSw889hy6rD8L7",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDid.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                format: VerificationMaterialFormatPeerDid.MULTIBASE
            ),
            new VerificationMaterialAuthentication
            (
                value: new Dictionary<string, string>
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_YgmA" },
                },
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            ),
            new VerificationMaterialAuthentication
            (
                value: Utils.ToJson(new Dictionary<string, string>
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_YgmA" },
                }),
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            )
        };
        return list.Select(p => new object[] { p });
    }

    public static IEnumerable<object[]> NotBase58Keys()
    {
        var list = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "x8xB2pv7cw8q1Pd0DacS",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDid.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "zx8xB2pv7cw8q1Pd0DacS",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                format: VerificationMaterialFormatPeerDid.MULTIBASE
            ),
        };
        return list.Select(p => new object[] { p });
    }

    public static IEnumerable<object[]> ShortKeys()
    {
        var list = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "ByHnpUCFb1vAfh9CFZ8ZkmUZguURW8nSw889hy6rD8",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDid.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                format: VerificationMaterialFormatPeerDid.MULTIBASE
            ),
            new VerificationMaterialAuthentication
            (
                value: new Dictionary<string, string>
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_Ygm" },
                },
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            ),
            new VerificationMaterialAuthentication
            (
                value: Utils.ToJson(new Dictionary<string, string>
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_Ygm" },
                }),
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            )
        };
        return list.Select(p => new object[] { p });
    }

    public static IEnumerable<object[]> LongKeys()
    {
        var list = new List<VerificationMaterialAuthentication>
        {
            new VerificationMaterialAuthentication
            (
                value: "ByHnpUCFb1vAfh9CFZ8ZkmUZguURW8nSw889hy6rD8L77",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDid.BASE58
            ),
            new VerificationMaterialAuthentication
            (
                value: "z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7VVV",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                format: VerificationMaterialFormatPeerDid.MULTIBASE
            ),
            new VerificationMaterialAuthentication
            (
                value: new Dictionary<string, string>
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_YgmA7" },
                },
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            ),
            new VerificationMaterialAuthentication
            (
                value: Utils.ToJson(new Dictionary<string, string>
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "owBhCbktDjkfS6PdQddT0D3yjSitaSysP3YimJ_YgmA7" },
                }),
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            )
        };
        return list.Select(p => new object[] { p });
    }


    public static IEnumerable<object[]> EmptyKeys()
    {
        var list = new List<VerificationMaterialAuthentication>()
        {
            new VerificationMaterialAuthentication(
                value: "",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2018,
                format: VerificationMaterialFormatPeerDid.BASE58
            ),
            new VerificationMaterialAuthentication(
                value: "",
                type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
                format: VerificationMaterialFormatPeerDid.MULTIBASE
            ),
            new VerificationMaterialAuthentication(
                value: new Dictionary<string, object>()
                {
                    { "kty", "OKP" },
                    { "crv", "Ed25519" },
                    { "x", "" },
                },
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            ),
            new VerificationMaterialAuthentication(
                value: JsonSerializer.Serialize(
                    new Dictionary<string, object>()
                    {
                        { "kty", "OKP" },
                        { "crv", "Ed25519" },
                        { "x", "" }
                    }
                ),
                type: VerificationMethodTypeAuthentication.JSON_WEB_KEY_2020,
                format: VerificationMaterialFormatPeerDid.JWK
            )
        };
        return list.Select(p => new object[] { p });
    }

    [Theory]
    [MemberData(nameof(ValidKeys))]
    public void TestCreateNumalgo0Positive(VerificationMaterialAuthentication key)
    {
        var peerDidAlgo0 = PeerDidCreator.CreatePeerDidNumalgo0(key);
        Assert.Equal("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V", peerDidAlgo0);
        Assert.True(PeerDidCreator.IsPeerDid(peerDidAlgo0));
    }
    
    [Theory]
    [MemberData(nameof(NotBase58Keys))]
    public void TestCreateNumalgo0MalformedInceptionKeyNotBase58Encoded(VerificationMaterialAuthentication key)
    {
        var ex = Assert.Throws<ArgumentException>(() => PeerDidCreator.CreatePeerDidNumalgo0(key));
        Assert.Matches("Invalid key: Invalid base58 encoding.*", ex.Message);
    }
    
    [Theory]
    [MemberData(nameof(ShortKeys))]
    public void TestCreateNumalgo0MalformedShortInceptionKey(VerificationMaterialAuthentication key)
    {
        var ex = Assert.Throws<ArgumentException>(() => PeerDidCreator.CreatePeerDidNumalgo0(key));
        Assert.Matches(new Regex("Invalid key.*"), ex.Message);
    }
    
    [Theory]
    [MemberData(nameof(LongKeys))]
    public void TestCreateNumalgo0MalformedLongInceptionKey(VerificationMaterialAuthentication key)
    {
        var ex = Assert.Throws<System.ArgumentException>(() => PeerDidCreator.CreatePeerDidNumalgo0(key));
        Assert.Matches("Invalid key.*", ex.Message);
    }
    
    [Theory]
    [MemberData(nameof(EmptyKeys))]
    public void TestCreateNumalgo0MalformedEmptyInceptionKey(VerificationMaterialAuthentication key)
    {
        var ex = Assert.Throws<System.ArgumentException>(() => PeerDidCreator.CreatePeerDidNumalgo0(key));
        string expectedError;
        switch (key.Format)
        {
            case VerificationMaterialFormatPeerDid.BASE58:
                expectedError = "Invalid key: Invalid base58 encoding.*";
                break;
            case VerificationMaterialFormatPeerDid.MULTIBASE:
                expectedError = "Invalid key: No transform part in multibase encoding.*";
                break;
            case VerificationMaterialFormatPeerDid.JWK:
                expectedError = "Invalid key.*";
                break;
            default:
                expectedError = string.Empty;
                break;
        }
    
        Assert.Matches(expectedError, ex.Message);
    }

    [Fact]
    public void TestCreateNumalgo0InvalidMulticodecPrefix()
    {
        var key = new VerificationMaterialAuthentication
        (
            value: "z78kqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V",
            type: VerificationMethodTypeAuthentication.ED25519_VERIFICATION_KEY_2020,
            format: VerificationMaterialFormatPeerDid.MULTIBASE
        );
        var ex = Assert.Throws<System.ArgumentException>(() => PeerDidCreator.CreatePeerDidNumalgo0(key));
        Assert.Matches("Invalid key: Prefix 1 not supported", ex.Message);
    }
}