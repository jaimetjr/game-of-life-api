using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace game_of_life_api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. CorrelationId={CorrelationId}", correlationId);

            var problem = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7807",
                Title = "An unexpected error occurred.",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            problem.Extensions["correlationId"] = correlationId;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }

}
