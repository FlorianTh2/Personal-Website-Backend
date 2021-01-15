using System.Linq;
using Microsoft.AspNetCore.Http;

namespace PersonalWebsiteBackend.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User==null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(a => a.Type == "id").Value;
        }
    }
}