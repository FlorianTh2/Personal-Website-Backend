using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Helpers
{
    public class FetchGithubRepositories
    {

        public static async Task<User> FetchCurrentUser(GithubSettings githubSettings)
        {
            var client = new GitHubClient(new ProductHeaderValue("personal-website"));
            // using personal access token
            // https://docs.github.com/en/github/authenticating-to-github/about-authentication-to-github
            // https://docs.github.com/en/github/authenticating-to-github/creating-a-personal-access-token
            var tokenAuth = new Credentials(githubSettings.GithubApiPersonalAccessToken);
            client.Credentials = tokenAuth;
            return await client.User.Current();
        }
        public static async Task<IEnumerable<Repository>> FetchAllGithubRepositoriesForCurrentUser(GithubSettings githubSettings)
        {
            var client = new GitHubClient(new ProductHeaderValue("personal-website"));
            var tokenAuth = new Credentials(githubSettings.GithubApiPersonalAccessToken);
            client.Credentials = tokenAuth;
            return await client.Repository.GetAllForCurrent();
        }
    }
}