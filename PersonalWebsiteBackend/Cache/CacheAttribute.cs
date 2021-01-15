using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Cache
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        // why not directly inject CacheService via contructor
        //    - because in that case every time one uses the attribute
        //    - you have to inject the CacheService (like: [Cache(IResponseCacheService responseCacheService, 600)])
        //        - that is somehow possible but "forbidden" and bad
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // check if request is cached
            // if true return
            var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();

            if (!cacheSettings.Enabled)
            {
                await next();
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            // output e.g. /api/v1/projects since we currently dont have queryparameters
            //    - (but this current method support possible query-parameter aswell)
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

            }

            // before controller
            var executedContext = await next();
            // after controller
            
            // get the value
            // cache the response

            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
                    TimeSpan.FromSeconds(_timeToLiveSeconds));
            }

        }

        // output e.g. /api/v1/projects
        // in the current implementation are possible query paramter included (but not needed)
        //    - in that case that method generates something like /api/v1/projects|queryParam1-queryParam1Value|queryParam1-queryParam1Value ...
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            // for this project currently not needed/important since we dont have any request-query-paramter
            foreach (var (key, value) in request.Query.OrderBy(a => a.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            var keyBuilderResult = keyBuilder.ToString();
            return keyBuilderResult;
        }
    }
}