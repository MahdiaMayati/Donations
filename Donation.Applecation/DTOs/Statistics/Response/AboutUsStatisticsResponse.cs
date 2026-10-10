namespace Donation.Application.DTOs.Statistics.Response;

public sealed class AboutUsStatisticsResponse
{
    public int BeneficiariesCount { get; init; }
    public int DonorsCount { get; init; }
    public int ActiveVolunteersCount { get; init; }
}
