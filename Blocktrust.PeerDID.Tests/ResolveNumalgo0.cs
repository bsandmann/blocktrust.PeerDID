using System.Text.RegularExpressions;
using Blocktrust.PeerDID.PeerDIDCreateResolve;
using Blocktrust.PeerDID.Types;

namespace Blocktrust.PeerDID.Tests;

public class ResolveNumalgo0
{
    [Fact]
    public void TestResolvePositiveDefault()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_MULTIBASE), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveMultibase()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_MULTIBASE), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Theory]
    [InlineData("did:key:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")]
    [InlineData("did:peer:0z6MkqRYqQiSBytw86Qbs2ZWUkGv22od935YF4s8M7V")]
    [InlineData("did:key:1z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")]
    [InlineData("did:peer:0z6MkqRYqQiSgvZQd0Bytw86Qbs2ZWUkGv22od935YF4s8M7V")]
    [InlineData("did:peer:0a6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")]
    public void TestResolveInvalidDids(string invalidDid)
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(invalidDid), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"), result.Errors.First().Message);
    }

    [Theory]
    [InlineData("did:peer:0z6666RYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")]
    [InlineData("did:peer:0z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc")]
    public void TestResolveInvalidKeys(string invalidDid)
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(invalidDid), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid key.*"), result.Errors.First().Message);
    }

    [Theory]
    [InlineData("did:peer:0z6LSbysY2xFMR")]
    [InlineData("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V&")]
    public void TestResolveInvalidKeyLengths(string invalidDid)
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(invalidDid), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"), result.Errors.First().Message);
    }
}