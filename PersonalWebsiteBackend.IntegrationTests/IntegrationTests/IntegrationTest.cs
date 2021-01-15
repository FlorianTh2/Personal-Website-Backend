using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using PersonalWebsiteBackend.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace PersonalWebsiteBackend.IntegrationTests.IntegrationTests
{
    // Intro to .net5 testing
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0
    public class IntegrationTest : IDisposable
    {
        // to get secrets -> we need  adminprofile
        // protected readonly IConfiguration _configuration;
        
        
        // attribute since maybe this factory needs additional configuration
        // e.g. init data
        //    - in that case you have to adjust the IWebHostBuilder inside the e.g. InMemoryWebApplicationFactory
        //    - e.g. client = _factory.WithWebHostBuilder(builder => /* *).CreateClient(/* */);
        //    - https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0
        protected readonly WebApplicationFactory<Startup> AppFactory;

        // attribute since its needed in each test
        protected readonly HttpClient TestClient;

        // - this tests use xUnit
        // - xUnit.net offers 3 basic methods for setup and cleanup code
        //    - Constructor and Dispose (shared setup/cleanup code without sharing object instances)
        //    - Class Fixtures (shared object instance across tests in a single class)
        //    - Collection Fixtures (shared object instances across multiple test classes)
        //
        // - here we use 1.: Constructor and Dispose
        // - because of that:
        // - xUnit.net creates a new instance of the test class for every test that is run
        //     so any code which is placed into the constructor of the test class will be run
        //     for every single test
        // - to make this work:
        //    For context cleanup, add the IDisposable interface to your test class, and put
        //    the cleanup code in the Dispose() method
        //
        // https://xunit.net/docs/shared-context
        // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0
        protected IntegrationTest()
        {
            // var builder = new ConfigurationBuilder().AddUserSecrets<PersonalWebsiteBackend.Startup>();
            // _configuration = builder.Build();

            AppFactory = new InMemoryWebApplicationFactory<Startup>();
            TestClient = AppFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest()
            {
                Email = "iamthemockadmin@iamthemockadmin.iamthemockadmin",
                Password = "AdminTestUserPW1234!!"
            });
            
            
            var loginResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            return loginResponse.Token;
        }

        protected async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Projects.Create, request);
            return (await response.Content.ReadFromJsonAsync<Response<ProjectResponse>>()).Data;
        }

        // this dispose function chain is called the dispose pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Problem: Nethertheless of TestClient.Dispose && Appfactory.Dispose,
        //    the same in-memory database is called
        // Solution:
        //    - 1. since the test-class constructor is called for each test (because of the way
        //        we defined xunit) and tries to create a new database, we can ensure that the database
        //        has a different name every time, so the context will bind to the new database with the new name
        //        (so we have a fresh database but i guess the old database continues to live)
        //    - 2. create and get scope to the Service DataContext and delete it
        //    ( - 3. https://docs.microsoft.com/en-us/ef/core/testing/testing-sample)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // access in-memory database
                using (var serviceScope = AppFactory.Services.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<DataContext>();
                    context.Database.EnsureDeleted();
                }

                TestClient.Dispose();
                AppFactory.Dispose();
            }
        }
    }
}