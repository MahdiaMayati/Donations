using Donation.Domain.Enums;

namespace Donation.Application.DTOs.DonationRequest.Request;

public sealed class UpdateDonationRequestStatusRequest
{
    public DonationRequestStatus Status { get; set; }
}
