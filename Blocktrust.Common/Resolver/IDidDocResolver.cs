namespace Blocktrust.Common.Resolver;

using Models.DidDoc;

public interface IDidDocResolver {
    Task<DidDoc?> Resolve(string did);
}