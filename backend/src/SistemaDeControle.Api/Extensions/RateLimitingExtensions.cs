using System.Threading.RateLimiting;

namespace SistemaDeControle.Api.Extensions;

public static class RateLimitingExtensions
{
    public const string LoginPolicy = "login";

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var permitLimit = configuration.GetValue("RateLimiting:LoginPermitLimit", 5);
        var windowSeconds = configuration.GetValue("RateLimiting:LoginWindowSeconds", 60);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Limite por IP de origem (resolvido via X-Forwarded-For, ver UseForwardedHeaders).
            options.AddPolicy(LoginPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        QueueLimit = 0,
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    status = StatusCodes.Status429TooManyRequests,
                    title = "Muitas tentativas",
                    detail = "Muitas tentativas em pouco tempo. Aguarde um minuto e tente novamente.",
                }, options: null, contentType: "application/problem+json", cancellationToken);
            };
        });

        return services;
    }
}
