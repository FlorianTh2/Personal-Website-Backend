using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Data
{
    public static class DataContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            var seedAdminProfile = new SeedAdminProfile();
            config.Bind(nameof(seedAdminProfile), seedAdminProfile);            
            
            var administratorRole = new IdentityRole(seedAdminProfile.IdentityRoleName);

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            var administrator = new ApplicationUser()
            {
                Email = seedAdminProfile.Email,
                UserName = seedAdminProfile.Email
            };

            if (userManager.Users.All(u => u.Email != administrator.Email))
            {
                await userManager.CreateAsync(administrator, seedAdminProfile.Password);
                await userManager.AddToRolesAsync(administrator, new[] {administratorRole.Name});
            }
        }

        public static async Task SeedSampleDataAsync(DataContext context)
        {
            // Seed, if necessary
            // if (!context.TodoLists.Any())
            // {
            //     context.TodoLists.Add(new TodoList
            //     {
            //         Title = "Shopping",
            //         Colour = Colour.Blue,
            //         Items =
            //         {
            //             new TodoItem { Title = "Apples", Done = true },
            //         }
            //     });
            //
            //     await context.SaveChangesAsync();
            // }
            
            // var client = new GitHubClient(new ProductHeaderValue("personal-website"));
            // // using personal access token
            // // https://docs.github.com/en/github/authenticating-to-github/about-authentication-to-github
            // // https://docs.github.com/en/github/authenticating-to-github/creating-a-personal-access-token
            // var tokenAuth = new Credentials("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");
            // client.Credentials = tokenAuth;
            // IEnumerable<Repository> repositories = await client.Repository.GetAllForCurrent();
            // foreach (Repository repository in repositories)
            // {
            //     Console.WriteLine(repository.CreatedAt);
            // }
        }
    }
}