namespace TRSAppUpdated.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
        T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
    }
}
