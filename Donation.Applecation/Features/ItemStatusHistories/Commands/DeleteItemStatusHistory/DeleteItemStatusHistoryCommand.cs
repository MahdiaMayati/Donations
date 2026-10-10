using MediatR;

namespace Donation.Application.Features.ItemStatusHistories.Commands.DeleteItemStatusHistory;

public sealed record DeleteItemStatusHistoryCommand(Guid Id) : IRequest<bool>;
