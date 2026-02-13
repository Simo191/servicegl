using System.Net;
using System.Text.Json;
using MultiServices.Application.Common.Models;
using MultiServices.Domain.Exceptions;

namespace MultiServices.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest,
                ApiResponse.Fail(ve.Errors.SelectMany(e => e.Value).ToList())),
            NotFoundException => (HttpStatusCode.NotFound,
                ApiResponse.Fail(exception.Message)),
            ForbiddenException => (HttpStatusCode.Forbidden,
                ApiResponse.Fail(exception.Message)),
            ConflictException => (HttpStatusCode.Conflict,
                ApiResponse.Fail(exception.Message)),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,
                ApiResponse.Fail("Unauthorized")),
            _ => (HttpStatusCode.InternalServerError,
                ApiResponse.Fail("An internal error occurred. Please try again later."))
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}
