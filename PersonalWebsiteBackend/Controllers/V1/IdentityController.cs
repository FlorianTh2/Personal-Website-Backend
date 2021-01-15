using System.Threading.Tasks;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Controllers.V1
{
    // {
    // "email": "test2@test.com",
    // "password": "Test1234!!"
    // }
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        // [HttpPost]
        // [Route(ApiRoutes.Identity.Register)]
        // public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userRegistrationRequest)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(new AuthFailedResponse
        //         {
        //             Errors = ModelState.Values.SelectMany(a => a.Errors.Select(b => b.ErrorMessage))
        //         });
        //     }
        //     var authResponse =
        //         await _identityService.RegisterAsync(userRegistrationRequest.Email, userRegistrationRequest.Password);
        //
        //     if (!authResponse.Success)
        //     {
        //         return BadRequest(new AuthFailedResponse
        //         {
        //             Errors = authResponse.Errors
        //         });
        //     }
        //     
        //     return Ok(new AuthSuccessResponse
        //     {
        //         Token = authResponse.Token,
        //         RefreshToken = authResponse.RefreshToken
        //     });
        // }
        
        [HttpPost]
        [Route(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            var authResponse =
                await _identityService.LoginAsync(userLoginRequest.Email, userLoginRequest.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost]
        [Route(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var authResponse =
                await _identityService.RefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });

        }
    }
}