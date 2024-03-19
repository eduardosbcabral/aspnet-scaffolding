namespace Scaffolding.Caching;

public interface ICacheProvider
{
    Task<T> Get<T>(string key);
    Task<bool> Update<T>(T obj, string key);
    Task<bool> Delete(string key);
}
