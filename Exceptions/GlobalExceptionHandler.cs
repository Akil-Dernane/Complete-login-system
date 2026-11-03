using Complete_Login_System.Exceptions.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Complete_Login_System.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode = exception is ApiException apiException
                ? apiException.StatusCode
                : StatusCodes.Status500InternalServerError;

            var title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status403Forbidden => "Forbidden",
                StatusCodes.Status404NotFound => "Resource Not Found",
                StatusCodes.Status409Conflict => "Conflict",
                _ => "Internal Server Error"
            };


            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Unhandled exception");
            }
            else
            {
                _logger.LogInformation(
                    "Request failed with status code {StatusCode}",
                    statusCode);
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode >= 500 
                ? "An unexpected error occurred."
                : exception.Message,
            };

            if (exception is IHasErrors hasErrors)
            {
                problem.Extensions["errors"] = hasErrors.Errors;
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }
}
