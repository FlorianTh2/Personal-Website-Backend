using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly IDateTime _dateTime;
        public DbSet<Project> Projects { get; set; }

        public DbSet<Document> Documents { get; set; }
        
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DataContext(DbContextOptions<DataContext> options, ICurrentUserService currentUserService, IDateTime dateTime)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        // needed to make PostTag have no id (otherwise: ef-error)
        //    - reason: its a intermediate table
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatorId = _currentUserService.UserId;
                        entry.Entity.CreatedOn = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdaterId = _currentUserService.UserId;
                        entry.Entity.UpdatedOn = _dateTime.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

    }
}