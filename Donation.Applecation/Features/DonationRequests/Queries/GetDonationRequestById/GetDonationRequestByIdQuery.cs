using Donation.Application.DTOs.DonationRequest.Response;
using MediatR;

namespace Donation.Application.Features.DonationRequests.Queries.GetDonationRequestById;

public sealed record GetDonationRequestByIdQuery(Guid Id) : IRequest<DonationRequestResponse?>;
