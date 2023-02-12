namespace Blocktrust.PeerDID.Exceptions;

public class MalformedPeerDidException : PeerDidException
{
    /// <summary>
    /// Raised if the peer DID to be resolved in not a valid peer DID.
    /// </summary>
    /// <param name="message">the detail message.</param>
    /// <param name="innerException">the cause of this.</param>
    public MalformedPeerDidException(string message, Exception innerException = null) : base("Invalid peer DID provided. " + message, innerException)
    {
    }
}