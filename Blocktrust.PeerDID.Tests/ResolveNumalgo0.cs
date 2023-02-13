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
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0));
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_MULTIBASE), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveBase58()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.BASE58);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_BASE58), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveMultibase()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.MULTIBASE);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_MULTIBASE), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveJWK()
    {
        var realValue = PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_0), VerificationMaterialFormatPeerDid.JWK);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_O_JWK), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolveUnsupportedDIDMethod()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => PeerDidResolver.ResolvePeerDid(new PeerDid("did:key:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")));
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveInvalidPeerDID()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6MkqRYqQiSBytw86Qbs2ZWUkGv22od935YF4s8M7V")));
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveUnsupportedNumalgoCode()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => PeerDidResolver.ResolvePeerDid(new PeerDid("did:key:1z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")));
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveMalformedBase58Encoding()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6MkqRYqQiSgvZQd0Bytw86Qbs2ZWUkGv22od935YF4s8M7V")));
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveUnsupportedTransformCode()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => { PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0a6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")); });
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveMalformedMulticodecEncoding()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => { PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6666RYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V")); });
        Assert.Matches(new Regex("Invalid peer DID provided.*Invalid key.*"), ex.Message);
    }

    [Fact]
    public void TestResolveInvalidKeyType()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => { PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc")); });
        Assert.Matches(new Regex("Invalid peer DID provided.*Invalid key.*"), ex.Message);
    }

    [Fact]
    public void TestResolveShortKey()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => { PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6LSbysY2xFMR")); });
        Assert.Matches("Invalid peer DID provided.*Does not match peer DID regexp.*", ex.Message);
    }

    [Fact]
    public void TestResolveLongKey()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => { PeerDidResolver.ResolvePeerDid(new PeerDid("did:peer:0z6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V&")); });
        Assert.Matches("Invalid peer DID provided.*Does not match peer DID regexp.*", ex.Message);
    }
    
}