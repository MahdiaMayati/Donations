using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemStatusHistory.Response;
using Donation.Application.Features.ItemStatusHistories.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemStatusHistories.Commands.CreateItemStatusHistory;

public sealed class CreateItemStatusHistoryCommandHandler
    : IRequestHandler<CreateItemStatusHistoryCommand, ItemStatusHistoryResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateItemStatusHistoryCommandHandler> _logger;

    public CreateItemStatusHistoryCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateItemStatusHistoryCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ItemStatusHistoryResponse> Handle(
        CreateItemStatusHistoryCommand request,
        CancellationToken cancellationToken)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException("Item not found.");
        }

        var now = DateTime.UtcNow;
        var oldStatus = request.OldStatus ?? item.SortingStatus;

        var history = new ItemStatusHistory
        {
            ItemId = item.Id,
            OldStatus = oldStatus,
            NewStatus = request.NewStatus,
            ChangedAt = now,
            ChangedByUserId = _currentUser.UserId
        };

        item.SortingStatus = request.NewStatus;
        item.SortedByUserId = _currentUser.UserId ?? item.SortedByUserId;
        item.UpdatedAt = now;

        _context.ItemStatusHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "ItemStatusHistory {HistoryId} created for Item {ItemId}",
            history.Id,
            item.Id);

        return ItemStatusHistoryMapper.Map(history);
    }
}
