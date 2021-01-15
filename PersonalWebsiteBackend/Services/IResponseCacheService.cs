using System;
using System.Threading.Tasks;

namespace PersonalWebsiteBackend.Services
{
    public interface IResponseCacheService
    {
        // cacheKey specifies under which key we want to store something
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive);

        Task<string> GetCachedResponseAsync(string cacheKey);
    }
}