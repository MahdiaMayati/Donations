using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;

namespace Donation.Application.Features.ItemCategories.Commands.CreateItemCategory;

public sealed record CreateItemCategoryCommand(string Name) : IRequest<ItemCategoryResponse>;
