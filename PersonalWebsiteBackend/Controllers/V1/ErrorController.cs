using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using PersonalWebsiteBackend.Contracts.V1.Responses;

namespace PersonalWebsiteBackend.Controllers.V1
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [AllowAnonymous]
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error; // Your exception
            var code = 500; // Internal Server Error by default

            if (exception is NotFoundException) code = 404; // Not Found
            else if (exception is UnauthorizedAccessException) code = 401; // Unauthorized
            else if (exception is Exception) code = 400; // Bad Request

            Response.StatusCode = code; // You can use HttpStatusCode enum instead

            return new BadRequestObjectResult(new ErrorResponse(new ErrorGeneralModel()
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            }));
        }
    }
}