namespace Blocktrust.PeerDID.Exceptions

{
    // ReSharper disable once InconsistentNaming
    public class MalformedPeerDIDDocException : PeerDidException
    {
        /// <summary>
        /// Raised if the resolved peer DID Doc to be resolved in not a valid peer DID.
        /// </summary>
        /// <param name="innerException">the cause of this.</param>
        public MalformedPeerDIDDocException(Exception innerException = null) : base("Invalid peer DID Doc", innerException)
        {
        }
    }
}