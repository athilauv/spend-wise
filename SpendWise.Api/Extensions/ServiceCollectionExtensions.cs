using SpendWise.Api.Middleware;

namespace SpendWise.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
