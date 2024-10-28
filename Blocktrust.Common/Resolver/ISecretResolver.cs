namespace Blocktrust.Common.Resolver;

using Models.Secrets;

public interface ISecretResolver
{
    Task<Secret?> FindKey(string kid);
    Task<HashSet<string>> FindKeys(List<string> kids);
    public Task AddKey(string kid, Secret secret);
}