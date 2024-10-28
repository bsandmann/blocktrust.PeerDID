namespace Blocktrust.Common.Exceptions;

public class DidUrlNotFoundException : DidDocException
{
    public DidUrlNotFoundException(string didUrl, string did)
        : base($"The DID URL '{didUrl}' not found in DID Doc '{did}'")
    {
    }
}