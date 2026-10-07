using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Item.Response;
using MediatR;

namespace Donation.Application.Features.Items.Queries.GetAllItems;

public sealed record GetAllItemsQuery(
    Guid? OrganizationId = null,
    Guid? DonationRequestId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<ItemResponse>>;
