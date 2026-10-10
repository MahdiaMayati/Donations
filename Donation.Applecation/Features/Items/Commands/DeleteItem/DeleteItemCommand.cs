using MediatR;

namespace Donation.Application.Features.Items.Commands.DeleteItem;

public sealed record DeleteItemCommand(Guid Id) : IRequest<bool>;
