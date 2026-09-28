using MediatR;

namespace Donation.Application.Features.Addresses.Commands.DeleteAddress;

public sealed record DeleteAddressCommand(int Id) : IRequest<bool>;
