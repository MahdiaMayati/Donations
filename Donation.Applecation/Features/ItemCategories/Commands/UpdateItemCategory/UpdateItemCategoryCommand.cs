using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;

namespace Donation.Application.Features.ItemCategories.Commands.UpdateItemCategory;

public sealed record UpdateItemCategoryCommand(Guid Id, string Name) : IRequest<ItemCategoryResponse?>;
