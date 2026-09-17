using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using TRSAppUpdated.Interfaces;

namespace TRSAppUpdated.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, byte> _allKeys = new();

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
            {
                return cachedValue;
            }

            var value = await factory();
            SetCache(key, value, absoluteExpiration, slidingExpiration);
            return value;
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
            {
                return cachedValue;
            }

            var value = factory();
            SetCache(key, value, absoluteExpiration, slidingExpiration);
            return value;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _allKeys.TryRemove(key, out _);
            _logger.LogInformation($"[Cache] Evicted key: {key}");
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _allKeys.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
        }

        private void SetCache<T>(string key, T value, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            if (value == null) return;

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromHours(2),
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(30)
            };

            options.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
            {
                _allKeys.TryRemove(evictedKey.ToString() ?? string.Empty, out _);
            });

            _memoryCache.Set(key, value, options);
            _allKeys.TryAdd(key, 0);
        }
    }
}
