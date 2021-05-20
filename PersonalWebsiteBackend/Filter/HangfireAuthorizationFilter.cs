using Hangfire.Dashboard;

namespace PersonalWebsiteBackend.Filter
{
    public class HangfireAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return context.GetHttpContext().User.Identity.IsAuthenticated;
        }
    }
}