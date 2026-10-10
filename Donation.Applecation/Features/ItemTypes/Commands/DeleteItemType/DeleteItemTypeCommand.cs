using MediatR;

namespace Donation.Application.Features.ItemTypes.Commands.DeleteItemType;

public sealed record DeleteItemTypeCommand(Guid Id) : IRequest<bool>;
