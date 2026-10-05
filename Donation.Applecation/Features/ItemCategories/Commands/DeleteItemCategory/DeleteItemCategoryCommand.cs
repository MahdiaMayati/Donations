using MediatR;

namespace Donation.Application.Features.ItemCategories.Commands.DeleteItemCategory;

public sealed record DeleteItemCategoryCommand(Guid Id) : IRequest<bool>;
