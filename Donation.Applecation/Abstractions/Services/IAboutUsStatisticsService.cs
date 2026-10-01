using Donation.Application.DTOs.Statistics.Response;

namespace Donation.Application.Abstractions.Services;

/// <summary>
/// Loads platform statistics for the public About Us page (cached in Infrastructure).
/// </summary>
public interface IAboutUsStatisticsService
{
    Task<AboutUsStatisticsResponse> GetAboutUsStatisticsAsync(CancellationToken cancellationToken = default);
}
