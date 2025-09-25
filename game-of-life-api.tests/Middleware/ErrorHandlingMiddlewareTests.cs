using FluentAssertions;
using game_of_life_api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Text.Json;

namespace game_of_life_api.tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task Should_PassThrough_When_NoException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var next = new RequestDelegate(_ => Task.CompletedTask);
        var logger = Substitute.For<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(next, logger);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        context.Response.Headers.Should().ContainKey("X-Correlation-Id");
    }

    [Fact]
    public async Task Should_ReturnProblemDetails_When_ExceptionThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var next = new RequestDelegate(_ => throw new InvalidOperationException("boom!"));
        var logger = Substitute.For<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(next, logger);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        context.Response.ContentType.Should().Be("application/problem+json");
        context.Response.Headers.Should().ContainKey("X-Correlation-Id");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var problem = JsonSerializer.Deserialize<ProblemDetails>(body)!;
        problem.Type.Should().Be("https://datatracker.ietf.org/doc/html/rfc7807");
        problem.Title.Should().Be("An unexpected error occurred.");
        problem.Detail.Should().Be("boom!");
        problem.Extensions.Should().ContainKey("correlationId");
    }
}
