using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemColor.Response;
using MediatR;

namespace Donation.Application.Features.ItemColors.Queries.GetAllItemColors;

public sealed record GetAllItemColorsQuery(
    Guid? ItemId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit) : IRequest<PaginatedResult<ItemColorResponse>>;
