using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Address.Response;
using MediatR;

namespace Donation.Application.Features.Addresses.Queries.GetAllAddresses;

public sealed record GetAllAddressesQuery(
    Guid? AreaId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<AddressResponse>>;
