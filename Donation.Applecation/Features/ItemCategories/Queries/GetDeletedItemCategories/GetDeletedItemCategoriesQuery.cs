using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;

namespace Donation.Application.Features.ItemCategories.Queries.GetDeletedItemCategories;

public sealed record GetDeletedItemCategoriesQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<ItemCategoryResponse>>;
