namespace Blocktrust.PeerDID.Tests;

using System.Text.RegularExpressions;
using Exceptions;
using PeerDIDCreateResolve;
using Types;

public class ResolveNumalgo0
{
    [Fact]
    public void TestResolvePositiveDefault()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_MULTIBASE), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveBase58()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.Base58);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_BASE58), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveMultibase()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_MULTIBASE), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveJWK()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.Jwk);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_JWK), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolveUnsupportedDIDMethod()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:key:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"), result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveInvalidPeerDID()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6MkqRYqQiSBytw86Qbs2ZWUkGv22od935YF4s8M7V"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"), result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveUnsupportedNumalgoCode()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:key:1z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"),  result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveMalformedBase58Encoding()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6MkqRYqQiSgvZQd0Bytw86Qbs2ZWUkGv22od935YF4s8M7V"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"),  result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveUnsupportedTransformCode()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0a6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"),VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"),  result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveMalformedMulticodecEncoding()
    {
        var result =PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6666RYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid key.*"),  result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveInvalidKeyType()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid key.*"),  result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveShortKey()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6LSbysY2xFMR"), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches("Does not match peer DID regexp.*",  result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveLongKey()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V&"),VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches("Does not match peer DID regexp.*",  result.Errors.First().Message);
    }
    
}