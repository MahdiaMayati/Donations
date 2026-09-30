using MediatR;

namespace Donation.Application.Features.Addresses.Commands.DeleteAddress;

public sealed record DeleteAddressCommand(Guid Id) : IRequest<bool>;
