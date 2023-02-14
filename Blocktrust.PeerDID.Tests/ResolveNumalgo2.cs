namespace Blocktrust.PeerDID.Tests;

using System.Text.RegularExpressions;
using Exceptions;
using PeerDIDCreateResolve;
using Types;

public class ResolveNumalgo2
{
    [Fact]
    public void TestResolvePositiveDefault()
    {
        string realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.MULTIBASE);
        Assert.Equal( Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveBase58()
    {
        string realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.BASE58);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_BASE58), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveMultibase()
    {
        string realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.MULTIBASE);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveJWK()
    {
        string realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2), VerificationMaterialFormatPeerDid.JWK);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_JWK), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveServiceIs2ElementsArray()
    {
        var realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_2_SERVICES), VerificationMaterialFormatPeerDid.MULTIBASE);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_2_SERVICES), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveNoService()
    {
        var realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_NO_SERVICES), VerificationMaterialFormatPeerDid.MULTIBASE);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_NO_SERVICES), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolvePositiveMinimalService()
    {
        var realValue =  PeerDidResolver.ResolvePeerDid(new PeerDid(Fixture.PEER_DID_NUMALGO_2_MINIMAL_SERVICES),VerificationMaterialFormatPeerDid.MULTIBASE);
        Assert.Equal(Fixture.RemoveWhiteSpace(Fixture.DID_DOC_NUMALGO_2_MULTIBASE_MINIMAL_SERVICES), Fixture.RemoveWhiteSpace(realValue));
    }

    [Fact]
    public void TestResolveUnsupportedNumalgoCode()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() =>
            PeerDidResolver.ResolvePeerDid(new PeerDid(
            "did:peer:1.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
            ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
            ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il19LHsidCI6ImV4YW1wbGUiLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdfV0="
        ), VerificationMaterialFormatPeerDid.MULTIBASE));
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveSigningMalformedBase58Encoding()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() =>
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSpSrLxbAh02SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Vz6MkqRYqQi0gvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il19LHsidCI6ImV4YW1wbGUiLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdfV0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE)
        );

        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveEncryptionMalformedBase58Encoding()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() =>
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSpSrLxbAh02SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il19LHsidCI6ImV4YW1wbGUiLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdfV0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE)
        );

        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveUnsupportedTransformCode()
    {
        Exception ex = Assert.Throws<MalformedPeerDidException>(() =>
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ea6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Va6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il19LHsidCI6ImV4YW1wbGUiLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdfV0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE));
        Assert.True(Regex.Match(ex.Message, "Invalid peer DID provided.*Does not match peer DID regexp.*").Success);
    }

    [Fact]
    public void TestResolveMalformedSigningMulticodecEncoding()
    {
        Exception ex = Assert.Throws<MalformedPeerDidException>(() =>
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Vz7MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SW3sidCI6ImRtIiwicyI6Imh0dHBzOi8vZXhhbXBsZS5jb20vZW5kcG9pbnQiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5Il19LHsidCI6ImV4YW1wbGUiLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludDIiLCJyIjpbImRpZDpleGFtcGxlOnNvbWVtZWRpYXRvciNzb21la2V5MiJdfV0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE));
        Assert.True(Regex.Match(ex.Message, "Invalid peer DID provided.*Invalid key.*").Success);
    }
    
    
    [Fact]
    public void TestResolveInvalidVerificationKeyType()
    {
        var exception = Assert.Throws<MalformedPeerDidException>(() =>
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Vz6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXX0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Invalid key.*"), exception.Message);
    }
    
    [Fact]
    public void TestResolveInvalidEncryptionKeyType()
    {
        var exception = Assert.Throws<MalformedPeerDidException>(() =>
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
                ".Ez6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXX0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Invalid key.*"), exception.Message);
    }
    
    [Fact]
    public void TestResolveMalformedServiceEncoding()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => 
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9\\\\GxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXX0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }

    [Fact]
    public void TestResolveMalformedService()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => 
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2.Ez6LSpSrLxbAhg2SHwKk7kwpsH7DM7QjFS5iK6qP87eViohud" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".SeyJ0IjoiZG0iLCJzIjo"
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Invalid service.*"), ex.Message);
    }
    
    [Fact]
    public void TestResolveInvalidPrefix()
    {
        var exception = Assert.Throws<MalformedPeerDidException>(() =>
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2" +
                ".Cz6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg" +
                ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0="
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), exception.Message);
    }

    [Fact]
    public void TestResolveShortSigningKey()
    {
        var exception = Assert.Throws<MalformedPeerDidException>(() =>
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2" +
                ".Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCc" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFp" +
                ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0"
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), exception.Message);
    }
    
   
    
    [Fact]
    public void TestResolveLongEncryptionKey()
    {
        var ex = Assert.Throws<MalformedPeerDidException>(() => 
        {
            PeerDidResolver.ResolvePeerDid(new PeerDid(
                "did:peer:2" +
                ".Ez6LSbysY2xFMRpGMhb7tFTLMpeuPRaqaWM1yECx2AtzE3KCccccc" +
                ".Vz6MkqRYqQiSgvZQdnBytw86Qbs2ZWUkGv22od935YF4s8M7V" +
                ".Vz6MkgoLTnTypo3tDRwCkZXSccTPHRLhF4ZnjhueYAFpEX6vg" +
                ".SeyJ0IjoiZG0iLCJzIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9lbmRwb2ludCIsInIiOlsiZGlkOmV4YW1wbGU6c29tZW1lZGlhdG9yI3NvbWVrZXkiXSwiYSI6WyJkaWRjb21tL3YyIiwiZGlkY29tbS9haXAyO2Vudj1yZmM1ODciXX0"
            ), VerificationMaterialFormatPeerDid.MULTIBASE);
        });
        Assert.Matches(new Regex("Invalid peer DID provided.*Does not match peer DID regexp.*"), ex.Message);
    }
}