using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octokit;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using Serilog;

namespace PersonalWebsiteBackend.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error; // Your exception
            var code = 500; // Internal Server Error by default

            if (exception is NotFoundException) code = 404; // Not Found
            else if (exception is UnauthorizedAccessException) code = 401; // Unauthorized
            else if (exception is Exception) code = 400; // Bad Request

            Response.StatusCode = code; // You can use HttpStatusCode enum instead

            var errorResponse = new ErrorResponse<ErrorGeneralModel>(new ErrorGeneralModel()
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            });

            _logger.LogError(string.Join(",", errorResponse.Errors.Select(a => a.Message)));
            _logger.LogError(string.Join(",", errorResponse.Errors.Select(a => a.StackTrace)));

            return new BadRequestObjectResult(errorResponse);
        }
    }
}