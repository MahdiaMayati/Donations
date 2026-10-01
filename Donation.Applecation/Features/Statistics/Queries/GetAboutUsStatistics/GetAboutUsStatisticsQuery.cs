using Donation.Application.DTOs.Statistics.Response;
using MediatR;

namespace Donation.Application.Features.Statistics.Queries.GetAboutUsStatistics;

public sealed record GetAboutUsStatisticsQuery : IRequest<AboutUsStatisticsResponse>;
