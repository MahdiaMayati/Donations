using Donation.Application.DTOs.Address.Response;
using MediatR;

namespace Donation.Application.Features.Addresses.Queries.GetAllAddresses;

public sealed record GetAllAddressesQuery(int? AreaId) : IRequest<IReadOnlyList<AddressResponse>>;
