namespace Blocktrust.PeerDID.Exceptions;

public class MalformedPeerDIDException : PeerDIDException
{
    /// <summary>
    /// Raised if the peer DID to be resolved in not a valid peer DID.
    /// </summary>
    /// <param name="message">the detail message.</param>
    /// <param name="innerException">the cause of this.</param>
    public MalformedPeerDIDException(string message, Exception innerException = null) : base("Invalid peer DID provided. " + message, innerException)
    {
    }
}