namespace Blocktrust.PeerDID.PeerDIDCreateResolve;

using Core;
using DIDDoc;
using Types;

public static class PeerDidCreator
{
    /// <summary>
    /// Checks if [peerDID] param matches PeerDID spec
    /// <a href="https://identity.foundation/peer-did-method-spec/index.html#matching-regex">Specification</a>
    /// </summary>
    /// <param name="peerDid">PeerDID to check</param>
    /// <returns>true if [peerDID] matches spec, otherwise false</returns>
    public static bool IsPeerDid(string peerDid)
    {
        string pattern = @"^did:peer:(([0](z)([1-9a-km-zA-HJ-NP-Z]{46,47}))" +
                         "|(2((.[AEVID](z)([1-9a-km-zA-HJ-NP-Z]{46,47}))+(.(S)[0-9a-zA-Z=]*)*)))$";
        return System.Text.RegularExpressions.Regex.IsMatch(peerDid, pattern);
    }

    /// <summary>
    /// Generates PeerDID according to the zero algorithm
    /// For this type of algorithm DIDDoc can be obtained from PeerDID
    /// <a href="https://identity.foundation/peer-did-method-spec/index.html#generation-method">Specification</a>
    /// </summary>
    /// <param name="inceptionKey">the key that creates the DID and authenticates when exchanging it with the first peer</param>
    /// <exception cref="ArgumentException">ArgumentException if the inceptionKey is not correctly encoded</exception>
    /// <returns>generated PeerDID</returns>
    public static string CreatePeerDidNumalgo0(VerificationMaterialAuthentication inceptionKey)
    {
        Validation.ValidateAuthenticationMaterialType(inceptionKey);
        return "did:peer:0" + PeerDidHelper.CreateMultibaseEncnumbasis(inceptionKey);
    }

    /// <summary>
    /// Generates PeerDID according to the second algorithm
    /// For this type of algorithm DIDDoc can be obtained from PeerDID
    /// <a href="https://identity.foundation/peer-did-method-spec/index.html#generation-method">Specification</a>
    /// </summary>
    /// <param name="encryptionKeys">[encryptionKeys] list of encryption keys</param>
    /// <param name="signingKeys">[signingKeys] list of signing keys</param>
    /// <param name="service"> [service] JSON string conforming to the DID specification</param>
    ///  /// <exception cref="ArgumentException">ArgumentException, if at least one of keys is not properly encoded,if service is not a valid JSON</exception>
    /// <returns>generated PeerDID</returns>
    public static string CreatePeerDidNumalgo2(
        List<VerificationMaterialAgreement> encryptionKeys,
        List<VerificationMaterialAuthentication> signingKeys,
        string? service)
    {
        encryptionKeys.ForEach(Validation.ValidateAgreementMaterialType);
        signingKeys.ForEach(Validation.ValidateAuthenticationMaterialType);

        string encodedEncryptionKeysStr = string.Join("", encryptionKeys
            .Select(PeerDidHelper.CreateMultibaseEncnumbasis)
            .Select(x => $".{Numalgo2Prefix.KEY_AGREEMENT}{x}"));
        string encodedSigningKeysStr = string.Join("", signingKeys
            .Select(PeerDidHelper.CreateMultibaseEncnumbasis)
            .Select(x => $".{Numalgo2Prefix.AUTHENTICATION}{x}"));
        string encodedService = string.IsNullOrWhiteSpace(service) ? "" : PeerDidHelper.EncodeService(service);

        return "did:peer:2" + encodedEncryptionKeysStr + encodedSigningKeysStr + encodedService;
    }
}