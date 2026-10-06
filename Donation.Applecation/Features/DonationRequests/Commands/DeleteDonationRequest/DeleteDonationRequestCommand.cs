using MediatR;

namespace Donation.Application.Features.DonationRequests.Commands.DeleteDonationRequest;

public sealed record DeleteDonationRequestCommand(Guid Id) : IRequest<bool>;
