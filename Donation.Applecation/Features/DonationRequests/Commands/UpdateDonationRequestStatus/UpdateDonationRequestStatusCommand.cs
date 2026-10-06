using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequestStatus;

public sealed record UpdateDonationRequestStatusCommand(
    Guid Id,
    DonationRequestStatus Status) : IRequest<DonationRequestResponse?>;
