using Donation.Application.DTOs.Address.Response;
using MediatR;

namespace Donation.Application.Features.Addresses.Queries.GetAllAddresses;

public sealed record GetAllAddressesQuery(Guid? AreaId) : IRequest<IReadOnlyList<AddressResponse>>;
