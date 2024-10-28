using System.Text.RegularExpressions;
using Blocktrust.PeerDID.PeerDIDCreateResolve;
using Blocktrust.PeerDID.Types;

namespace Blocktrust.PeerDID.Tests;

public class ResolveNumalgo2
{
    [Fact]
    public void TestResolvePositiveDefault()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveMultibase()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveServiceIs2ElementsArray()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_2_SERVICES), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveNoService()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Fact]
    public void TestResolvePositiveMinimalService()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES), Fixture.RemoveWhiteSpace(realValue.Value));
    }

    [Theory]
    [InlineData("did:peer:1.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0")]
    [InlineData("did:peer:2.Ez6LSpSrLxbAh02SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Vz6MkqRYqQi0gvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0")]
    [InlineData("did:peer:2.Ea6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud.Va6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0")]
    public void TestResolveInvalidDids(string invalidDid)
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(invalidDid), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Does not match peer DID regexp.*"), result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveMalformedSigningKey()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(
            "did:peer:2.Ez6LSpSrLxbAh02SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
            ".Vz6MkqRYqQi0gvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
            ".SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0"
        ), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid key.*"), result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveInvalidServiceEndpoint()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCJ9"
        ), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid service.*"), result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveMalformedService()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(
            "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc.Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V.SW3sidCI6ImRtIiwicyI6Imh0dHA="
        ), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid service.*"), result.Errors.First().Message);
    }

    [Fact]
    public void TestResolveInvalidKeyType()
    {
        var result = PeerDidResolver.ResolvePeerDid(new PeerDid(
            "did:peer:2.Vz6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
            ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
            ".SeyJ0IjoiZG0iLCJzIjp7InVyaSI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQifX0"
        ), VerificationMaterialFormatPeerDid.Multibase);
        Assert.Matches(new Regex("Invalid key.*"), result.Errors.First().Message);
    }
}