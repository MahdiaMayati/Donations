using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Donations.Extensions;

public static class RateLimitingPolicies
{
    public const string AboutUsStatistics = "about-us-statistics";
    public const string DonationSubmission = "donation-submission";
}

public static class RateLimitingExtensions
{
    public const string AboutUsStatisticsPolicy = RateLimitingPolicies.AboutUsStatistics;

    /// <summary>
    /// Fixed-window limiters for public About Us stats (60/min) and donation submission (10/min).
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
                        errors = new { detail = "Rate limit exceeded." },
                        data = (object?)null
                    },
                    cancellationToken);
            };

            options.AddPolicy(RateLimitingPolicies.AboutUsStatistics, httpContext =>
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

            options.AddPolicy(RateLimitingPolicies.DonationSubmission, httpContext =>
            {
                var partitionKey = httpContext.User.Identity?.Name
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }
}
