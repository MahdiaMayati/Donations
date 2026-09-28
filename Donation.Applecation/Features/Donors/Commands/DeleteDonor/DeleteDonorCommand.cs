using MediatR;

namespace Donation.Application.Features.Donors.Commands.DeleteDonor;

public sealed record DeleteDonorCommand(int Id) : IRequest<bool>;
