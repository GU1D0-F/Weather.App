using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Oquesobra.Weather.Service.Cache;

namespace Oquesobra.Weather.Service.Api;

/// <summary>
/// Static class that contains extension methods for configuring the cache service.
/// </summary>
public static class Cache
{

    /// <summary>
    /// Adds cache services to the service collection.
    /// Registers the <see cref="IMemoryCache"/> service and the <see cref="ICacheService"/> implementation.
    /// </summary>
    /// <param name="serviceCollection">The service collection where the cache services will be registered.</param>
    public static void AddCache(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<ICacheService, CacheService>();
    }
}