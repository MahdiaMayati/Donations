using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;

namespace Donation.Application.Features.ItemCategories.Queries.GetItemCategoryById;

public sealed record GetItemCategoryByIdQuery(Guid Id) : IRequest<ItemCategoryResponse?>;
