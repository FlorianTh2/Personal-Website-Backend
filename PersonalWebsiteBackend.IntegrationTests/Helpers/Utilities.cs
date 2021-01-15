using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace PersonalWebsiteBackend.IntegrationTests.Helpers
{
    public static class Utilities
    {
        public static async Task InitializeDbForTests(DataContext db,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // db.Projects.Add(GetSeedingAdminUser());
            // db.SaveChanges();

            var administratorRole = new IdentityRole("Administrator");
            ApplicationUser adminTestUser = new ApplicationUser()
            {
                Email = "iamthemockadmin@iamthemockadmin.iamthemockadmin",
                UserName = "iamthemockadmin@iamthemockadmin.iamthemockadmin",
            };
            string adminTestPw = "AdminTestUserPW1234!!";

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }
            
            if (userManager.Users.All(u => u.Email != adminTestUser.Email))
            {
                await userManager.CreateAsync(adminTestUser, adminTestPw);
                await userManager.AddToRolesAsync(adminTestUser, new[] {administratorRole.Name});
            }
        }
    }
}