using Donation.Application.Features.Statistics.Queries.GetAboutUsStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Donation.Api.Controllers;

[ApiExplorerSettings(GroupName = "system")]
[ApiController]
[Route("api/v1/statistics")]
public sealed class StatisticsController : BaseController
{
    private readonly ISender _sender;

    public StatisticsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Public About Us platform statistics (cached 24h). Rate limited: 60 req/min per IP.
    /// </summary>
    [HttpGet("about-us")]
    [AllowAnonymous]
    [EnableRateLimiting("about-us-statistics")]
    public async Task<IActionResult> GetAboutUs(CancellationToken cancellationToken)
    {
        var stats = await _sender.Send(new GetAboutUsStatisticsQuery(), cancellationToken);
        return CustomResponse(stats, "Dynamic platform statistics retrieved successfully.");
    }
}
