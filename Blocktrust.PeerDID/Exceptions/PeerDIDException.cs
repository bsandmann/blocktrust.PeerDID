namespace Blocktrust.PeerDID.Exceptions;

public class PeerDidException : Exception
{
    /// <summary>
    /// The base class for all PeerDID errors and exceptions.
    /// </summary>
    /// <param name="message">the detail message.</param>
    /// <param name="innerException"> the cause of this.</param>
    public PeerDidException(string message, Exception innerException = null) : base(message, innerException)
    {
    }
}