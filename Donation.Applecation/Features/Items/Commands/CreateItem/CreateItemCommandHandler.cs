using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Item.Response;
using Donation.Application.Features.Items.Common;
using Donation.Domain.Entities;
using Donation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Items.Commands.CreateItem;

public sealed class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ItemResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateItemCommandHandler> _logger;

    public CreateItemCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateItemCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ItemResponse> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var donationRequest = await _context.DonationRequests
            .AsNoTracking()
            .Where(d => d.Id == request.DonationRequestId)
            .Select(d => new { d.Id, d.OrganizationId })
            .FirstOrDefaultAsync(cancellationToken);

        if (donationRequest is null)
        {
            throw new NotFoundException("Donation request not found.");
        }

        var organizationId = request.OrganizationId is Guid oid && oid != Guid.Empty
            ? oid
            : donationRequest.OrganizationId;

        if (organizationId != donationRequest.OrganizationId)
        {
            throw new BusinessRuleException(
                "OrganizationId must match the donation request organization.");
        }

        var organizationExists = await _context.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Id == organizationId && !o.IsDeleted, cancellationToken);
        if (!organizationExists)
        {
            throw new NotFoundException("Organization not found.");
        }

        await ItemCatalogGuard.EnsureReferencesAsync(
            _context,
            request.ItemTypeId,
            request.MaterialId,
            request.StorageLocationId,
            request.SortedByUserId,
            cancellationToken);

        var barcode = ItemCatalogGuard.NormalizeBarcode(request.Barcode);
        await ItemCatalogGuard.EnsureBarcodeUniqueAsync(_context, barcode, null, cancellationToken);

        var now = DateTime.UtcNow;
        var item = new Item
        {
            OrganizationId = organizationId,
            DonationRequestId = request.DonationRequestId,
            ItemTypeId = request.ItemTypeId,
            MaterialId = request.MaterialId is Guid mid && mid != Guid.Empty ? mid : null,
            StorageLocationId = request.StorageLocationId is Guid sid && sid != Guid.Empty ? sid : null,
            SortedByUserId = request.SortedByUserId is Guid uid && uid != Guid.Empty ? uid : null,
            Barcode = barcode,
            TargetGender = request.TargetGender,
            AgeGroup = request.AgeGroup,
            Size = request.Size.Trim(),
            Season = request.Season,
            Condition = request.Condition,
            SortingStatus = ItemSortingStatus.PendingReview,
            AvailabilityStatus = request.AvailabilityStatus ?? ItemAvailabilityStatus.PendingReview,
            SorterNotes = ItemCatalogGuard.NormalizeOptionalText(request.SorterNotes, 2000),
            ReceivedAt = request.ReceivedAt,
            CreatedAt = now,
            IsDeleted = false
        };

        item.StatusHistory.Add(new ItemStatusHistory
        {
            OldStatus = null,
            NewStatus = ItemSortingStatus.PendingReview,
            ChangedAt = now,
            ChangedByUserId = _currentUser.UserId
        });

        _context.Items.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item created with Id {ItemId}", item.Id);
        return ItemMapper.Map(item);
    }
}
