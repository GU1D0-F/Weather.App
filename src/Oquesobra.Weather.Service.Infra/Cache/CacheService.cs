using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Cache
{
    public class CacheService(IMemoryCache memoryCache) : ICacheService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItemCallback, TimeSpan cacheDuration)
        {
            if (_memoryCache.TryGetValue(key, out T cachedItem))
                return cachedItem;

            var item = await getItemCallback();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };

            _memoryCache.Set(key, item, cacheEntryOptions);

            return item;
        }
    }
}
