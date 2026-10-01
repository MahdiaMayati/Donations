using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;

namespace Donation.Application.Features.ItemCategories.Commands.RestoreItemCategory;

public sealed record RestoreItemCategoryCommand(Guid Id) : IRequest<ItemCategoryResponse?>;
