using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Item.Response;
using Donation.Application.Features.Items.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Items.Commands.UpdateItem;

public sealed class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateItemCommandHandler> _logger;

    public UpdateItemCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateItemCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ItemResponse?> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Items
            .Include(i => i.Photos)
            .Include(i => i.ItemColors)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item is null)
        {
            return null;
        }

        await ItemCatalogGuard.EnsureReferencesAsync(
            _context,
            request.ItemTypeId,
            request.MaterialId,
            request.StorageLocationId,
            request.SortedByUserId,
            cancellationToken);

        var barcode = ItemCatalogGuard.NormalizeBarcode(request.Barcode);
        await ItemCatalogGuard.EnsureBarcodeUniqueAsync(_context, barcode, item.Id, cancellationToken);

        var now = DateTime.UtcNow;
        var previousSortingStatus = item.SortingStatus;

        item.ItemTypeId = request.ItemTypeId;
        item.MaterialId = request.MaterialId is Guid mid && mid != Guid.Empty ? mid : null;
        item.StorageLocationId = request.StorageLocationId is Guid sid && sid != Guid.Empty ? sid : null;
        item.SortedByUserId = request.SortedByUserId is Guid uid && uid != Guid.Empty ? uid : null;
        item.Barcode = barcode;
        item.TargetGender = request.TargetGender;
        item.AgeGroup = request.AgeGroup;
        item.Size = request.Size.Trim();
        item.Season = request.Season;
        item.Condition = request.Condition;
        item.AvailabilityStatus = request.AvailabilityStatus;
        item.SorterNotes = ItemCatalogGuard.NormalizeOptionalText(request.SorterNotes, 2000);
        item.ReceivedAt = request.ReceivedAt;
        item.UpdatedAt = now;

        if (previousSortingStatus != request.SortingStatus)
        {
            item.SortingStatus = request.SortingStatus;
            ItemStatusHistoryWriter.Append(
                _context,
                item,
                previousSortingStatus,
                request.SortingStatus,
                _currentUser.UserId,
                now);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item updated with Id {ItemId}", item.Id);
        return ItemMapper.Map(item);
    }
}
