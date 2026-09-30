using Donation.Application.DTOs.Address.Response;
using MediatR;

namespace Donation.Application.Features.Addresses.Queries.GetAddressById;

public sealed record GetAddressByIdQuery(Guid Id) : IRequest<AddressResponse?>;
