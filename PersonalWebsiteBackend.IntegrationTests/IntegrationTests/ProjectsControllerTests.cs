using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using PersonalWebsiteBackend.Domain;
using FluentAssertions;
using Xunit;

namespace PersonalWebsiteBackend.IntegrationTests.IntegrationTests
{
    public class ProjectsControllerTests : IntegrationTest
    {
        public ProjectsControllerTests() : base()
        {
        }


        // naming convention: methodname_Scenario_ExpectedReturn
        //
        // content of each test: "The triple A"
        //    - 1. Arrange
        //    - 2. Act
        //    - 3. Assert
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Projects.GetAll);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<PagedResponse<Project>>()).Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInTheDatabase()
        {
            // Assert
            await AuthenticateAsync();
            var createdProject = await CreateProjectAsync(new CreateProjectRequest()
            {
                Name = "Test Project",
                Tags = new []{"testtag"}
            });

            // Act
            var response =
                await TestClient.GetAsync(ApiRoutes.Projects.Get.Replace("{projectId}", createdProject.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var string1 = await response.Content.ReadAsStringAsync();
            var returnedProject = await response.Content.ReadFromJsonAsync<Response<ProjectResponse>>();
            returnedProject.Data.Id.Should().Be(createdProject.Id);
            returnedProject.Data.Name.Should().Be("Test Project");
            returnedProject.Data.Tags.Single().Name.Should().Be("testtag");
            
        }
    }
}