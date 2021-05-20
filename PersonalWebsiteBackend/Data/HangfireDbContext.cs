using Microsoft.EntityFrameworkCore;

namespace PersonalWebsiteBackend.Data
{
    public class HangfireDbContext : DbContext
    {
        public HangfireDbContext(DbContextOptions<HangfireDbContext> options) : base(options)
        {
        }
    }
}