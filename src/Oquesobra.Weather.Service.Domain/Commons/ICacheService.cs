using System;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItemCallback, TimeSpan cacheDuration);
    }
}
