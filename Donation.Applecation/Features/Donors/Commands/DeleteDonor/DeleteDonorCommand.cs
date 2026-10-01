using MediatR;

namespace Donation.Application.Features.Donors.Commands.DeleteDonor;

public sealed record DeleteDonorCommand(Guid Id) : IRequest<bool>;
