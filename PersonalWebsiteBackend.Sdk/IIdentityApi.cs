using System.Threading.Tasks;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using Refit;

namespace PersonalWebsiteBackend.Sdk
{
    // - the mapping is not archieved by same name like the controller-action (this is
    //    not the case and demonstrated here since the names differ)
    // - the mapping is archieved through the routing-path
    public interface IIdentityApi
    {
        // we cant use ApiRoutes since Refit cant handle constants
        [Post("/api/v1/identity/register")]
        Task<ApiResponse<AuthSuccessResponse>> RegisterAsync([Body] UserRegistrationRequest registrationRequest);
        
        [Post("/api/v1/identity/login")]
        Task<ApiResponse<AuthSuccessResponse>> LoginAsync([Body] UserLoginRequest loginRequest);
        
        [Post("/api/v1/identity/refresh")]
        Task<ApiResponse<AuthSuccessResponse>> RefreshAsync([Body] RefreshTokenRequest refreshRequest);
    }
}