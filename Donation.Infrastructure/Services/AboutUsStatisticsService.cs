using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Statistics.Response;
using Donation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Donation.Infrastructure.Services;

public sealed class AboutUsStatisticsService : IAboutUsStatisticsService
{
    public const string CacheKey = "statistics:about-us";
    public static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    private readonly IMemoryCache _cache;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<AboutUsStatisticsService> _logger;

    public AboutUsStatisticsService(
        IMemoryCache cache,
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<AboutUsStatisticsService> logger)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<AboutUsStatisticsResponse> GetAboutUsStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out AboutUsStatisticsResponse? cached) && cached is not null)
        {
            return cached;
        }

        var stats = await LoadFromDatabaseAsync(cancellationToken);

        _cache.Set(
            CacheKey,
            stats,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

        _logger.LogInformation(
            "About Us statistics refreshed and cached for {Hours}h. Donors={Donors}, Beneficiaries={Beneficiaries}, ActiveVolunteers={Volunteers}, DonatedItems={Items}",
            CacheDuration.TotalHours,
            stats.DonorsCount,
            stats.BeneficiariesCount,
            stats.ActiveVolunteersCount,
            stats.DonatedItemsCount);

        return stats;
    }

    private async Task<AboutUsStatisticsResponse> LoadFromDatabaseAsync(CancellationToken cancellationToken)
    {
        // Separate DbContexts so counts can run concurrently (DbContext is not thread-safe).
        await using var donorsContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using var beneficiariesContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using var volunteersContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Raw COUNT avoids entity/schema drift (Guid ids, nvarchar Status, soft-delete columns).
        var donorsTask = CountAsync(
            donorsContext,
            "SELECT COUNT(*) AS [Value] FROM Donors WHERE IsDeleted = 0",
            cancellationToken);

        var beneficiariesTask = CountAsync(
            beneficiariesContext,
            "SELECT COUNT(*) AS [Value] FROM Beneficiaries WHERE IsDeleted = 0",
            cancellationToken);

        var activeVolunteersTask = CountAsync(
            volunteersContext,
            "SELECT COUNT(*) AS [Value] FROM Volunteers WHERE Status = N'Active' AND IsDeleted = 0",
            cancellationToken);

        await Task.WhenAll(donorsTask, beneficiariesTask, activeVolunteersTask);

        // Donation / donated-items module is not in the domain yet — return 0 (never null).
        const int donatedItemsCount = 0;

        return new AboutUsStatisticsResponse
        {
            DonatedItemsCount = donatedItemsCount,
            BeneficiariesCount = await beneficiariesTask,
            DonorsCount = await donorsTask,
            ActiveVolunteersCount = await activeVolunteersTask
        };
    }

    private static Task<int> CountAsync(
        AppDbContext context,
        string sql,
        CancellationToken cancellationToken)
        => context.Database
            .SqlQueryRaw<int>(sql)
            .SingleAsync(cancellationToken);
}
