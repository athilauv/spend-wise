using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Authentication;

namespace SpendWise.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            // Let cancelled requests propagate without writing an error response.
            if (exception is OperationCanceledException)
            {
                throw;
            }

            if (context.Response.HasStarted)
            {
                logger.LogError(exception, "Unhandled exception after response started.");
                throw;
            }

            var (status, title, detail, errors) = Map(exception);

            if (status >= 500)
                logger.LogError(exception, "Unhandled server error. TraceId: {TraceId}", context.TraceIdentifier);
            else
                logger.LogInformation("Request failed with {StatusCode}: {Message}", status, exception.Message);

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;
            if (errors is not null)
                problem.Extensions["errors"] = errors;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static (int Status, string Title, string Detail, IDictionary<string, string[]>? Errors)
        Map(Exception exception) => exception switch
    {
        ValidationException validation => (
            StatusCodes.Status400BadRequest,
            "Validation failed",
            "One or more request fields are invalid.",
            validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray())),
        NotFoundException notFound => (404, "Resource not found", notFound.Message, null),
        ConflictException conflict => (409, "Conflict", conflict.Message, null),
        DuplicateEmailException duplicate => (409, "Conflict", duplicate.Message, null),
        InvalidCredentialsException => (401, "Unauthorized", "Email or password is incorrect.", null),
        UnauthorizedAccessException => (403, "Forbidden", "You are not allowed to perform this operation.", null),
        ArgumentException argument => (400, "Invalid request", argument.Message, null),
        InvalidOperationException invalid => (StatusCodes.Status400BadRequest, "Invalid request", invalid.Message, null),
        _ => (500, "Server error", "An unexpected error occurred. Use the traceId when contacting support.", null)
    };
}



