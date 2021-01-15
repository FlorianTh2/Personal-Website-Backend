using System.Net.Http;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Contracts.V1;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PersonalWebsiteBackend.IntegrationTests.IntegrationTests
{
    public class xUnitIntro
    {

        private readonly HttpClient _client;

        public xUnitIntro()
        {
            // is the in-memory-server
            // the in-memory-server (stores in a property) gets created with the CreateClient()-Method
            // the CreateClient()-Method
            //    - 1. initializes the in-memory-server
            //    - 2. creates the client
            var appFactory = new WebApplicationFactory<Startup>();
            // initializes the server + creates the corresponding client
            _client = appFactory.CreateClient();
        }
        
        // [Fact]
        public async Task Test1()
        {
            var response = await _client.GetAsync(ApiRoutes.Projects.Get.Replace("{projectId}", "1"));
        }
    }
}