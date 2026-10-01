using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Donations.Extensions;

public static class RateLimitingExtensions
{
    public const string AboutUsStatisticsPolicy = "about-us-statistics";

    /// <summary>
    /// Fixed-window limiter: 60 requests / minute / client IP (public About Us stats).
    /// </summary>
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        success = false,
                        message = "Too many requests. Please try again later.",
                        errors = new { detail = "Rate limit exceeded (60 requests per minute)." },
                        data = (object?)null
                    },
                    cancellationToken);
            };

            options.AddPolicy(AboutUsStatisticsPolicy, httpContext =>
            {
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }
}
