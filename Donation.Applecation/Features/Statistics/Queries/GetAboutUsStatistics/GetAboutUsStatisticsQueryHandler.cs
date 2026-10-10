using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Statistics.Response;
using MediatR;

namespace Donation.Application.Features.Statistics.Queries.GetAboutUsStatistics;

public sealed class GetAboutUsStatisticsQueryHandler
    : IRequestHandler<GetAboutUsStatisticsQuery, AboutUsStatisticsResponse>
{
    private readonly IAboutUsStatisticsService _statisticsService;

    public GetAboutUsStatisticsQueryHandler(IAboutUsStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public Task<AboutUsStatisticsResponse> Handle(
        GetAboutUsStatisticsQuery request,
        CancellationToken cancellationToken)
        => _statisticsService.GetAboutUsStatisticsAsync(cancellationToken);
}
