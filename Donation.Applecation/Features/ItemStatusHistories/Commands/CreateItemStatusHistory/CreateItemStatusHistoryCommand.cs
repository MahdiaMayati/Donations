using Donation.Application.DTOs.ItemStatusHistory.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.ItemStatusHistories.Commands.CreateItemStatusHistory;

public sealed record CreateItemStatusHistoryCommand(
    Guid ItemId,
    ItemSortingStatus NewStatus,
    ItemSortingStatus? OldStatus) : IRequest<ItemStatusHistoryResponse>;
