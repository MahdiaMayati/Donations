using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Item.Response;
using Donation.Application.Features.Items.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Items.Commands.UpdateItemSortingStatus;

public sealed class UpdateItemSortingStatusCommandHandler
    : IRequestHandler<UpdateItemSortingStatusCommand, ItemResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateItemSortingStatusCommandHandler> _logger;

    public UpdateItemSortingStatusCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateItemSortingStatusCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ItemResponse?> Handle(
        UpdateItemSortingStatusCommand request,
        CancellationToken cancellationToken)
    {
        var item = await _context.Items
            .Include(i => i.Photos)
            .Include(i => i.ItemColors)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var previous = item.SortingStatus;

        if (previous != request.SortingStatus)
        {
            item.SortingStatus = request.SortingStatus;
            item.SortedByUserId = _currentUser.UserId ?? item.SortedByUserId;
            item.UpdatedAt = now;

            ItemStatusHistoryWriter.Append(
                _context,
                item,
                previous,
                request.SortingStatus,
                _currentUser.UserId,
                now);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Item {ItemId} sorting status changed from {OldStatus} to {NewStatus}",
                item.Id,
                previous,
                request.SortingStatus);
        }

        return ItemMapper.Map(item);
    }
}
