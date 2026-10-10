using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.DonationRequest.Request;
using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Application.Features.DonationRequests.Common;
using Donation.Domain.Entities;
using Donation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequest;

public sealed class UpdateDonationRequestCommandHandler
    : IRequestHandler<UpdateDonationRequestCommand, DonationRequestResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateDonationRequestCommandHandler> _logger;

    public UpdateDonationRequestCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateDonationRequestCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<DonationRequestResponse?> Handle(
        UpdateDonationRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        DonationRequest entity;
        Dictionary<Guid, IReadOnlyList<string>> itemPhotos;
        try
        {
            entity = await _context.DonationRequests
                .Include(d => d.Donor)
                .Include(d => d.Photos)
                .Include(d => d.Items)
                    .ThenInclude(i => i.Photos)
                .Include(d => d.Items)
                    .ThenInclude(i => i.ItemColors)
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException("Donation request not found.");

            await EnsureCanModifyAsync(entity, cancellationToken);

            if (entity.Status != DonationRequestStatus.Submitted)
            {
                throw new BusinessRuleException(
                    "Donation request details can only be updated while status is Submitted.");
            }

            if (request.DeliveryMethod == DeliveryMethod.VolunteerPickup)
            {
                await EnsurePickupAddressAsync(
                    request.PickupAddressId!.Value,
                    entity.Donor.UserId,
                    cancellationToken);
            }

            await DonationRequestCatalogGuard.EnsureCatalogReferencesAsync(
                _context, request.Items, cancellationToken);
            await DonationRequestCatalogGuard.EnsureBarcodesUniqueAsync(
                _context,
                request.Items.Select(i => i.Barcode),
                request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value),
                cancellationToken);

            var now = DateTime.UtcNow;
            entity.DeliveryMethod = request.DeliveryMethod;
            entity.PickupAddressId = request.DeliveryMethod == DeliveryMethod.VolunteerPickup
                ? request.PickupAddressId
                : null;
            entity.Description = request.Description.Trim();
            entity.EstimatedItemCount = request.EstimatedItemCount;
            entity.UpdatedAt = now;

            itemPhotos = SyncItems(entity, request.Items, now);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("DonationRequest {DonationRequestId} updated", entity.Id);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        await DonationRequestPhotoPersistence.TrySyncUpdatePhotosAsync(
            _context,
            entity,
            request.RequestPhotoUrls,
            itemPhotos,
            _logger,
            cancellationToken);

        return await LoadResponseAsync(entity.Id, cancellationToken);
    }

    private Dictionary<Guid, IReadOnlyList<string>> SyncItems(
        DonationRequest entity,
        IReadOnlyList<DonationRequestItemRequest> itemRequests,
        DateTime now)
    {
        var activeItems = entity.Items.Where(i => !i.IsDeleted).ToDictionary(i => i.Id);
        var keptIds = new HashSet<Guid>();
        var itemPhotos = new Dictionary<Guid, IReadOnlyList<string>>();

        foreach (var itemRequest in itemRequests)
        {
            if (itemRequest.Id is Guid existingId && existingId != Guid.Empty)
            {
                if (!activeItems.TryGetValue(existingId, out var existing))
                {
                    throw new NotFoundException($"Item '{existingId}' was not found on this donation request.");
                }

                DonationRequestItemFactory.ApplyItemFields(existing, itemRequest, now);
                DonationRequestItemFactory.ApplyColors(existing, itemRequest.ColorIds);
                keptIds.Add(existing.Id);
                itemPhotos[existing.Id] = itemRequest.PhotoUrls;
                continue;
            }

            var created = DonationRequestItemFactory.CreateItem(
                itemRequest,
                entity.OrganizationId,
                now,
                _currentUser.UserId);
            entity.Items.Add(created);
            keptIds.Add(created.Id);
            itemPhotos[created.Id] = itemRequest.PhotoUrls;
        }

        foreach (var orphan in activeItems.Values.Where(i => !keptIds.Contains(i.Id)))
        {
            orphan.IsDeleted = true;
            orphan.DeletedAt = now;
            orphan.UpdatedAt = now;
        }

        return itemPhotos;
    }

    private async Task EnsureCanModifyAsync(DonationRequest entity, CancellationToken cancellationToken)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        var isOwner = entity.Donor.UserId == _currentUser.UserId;
        if (isOwner)
        {
            return;
        }

        var userOrgId = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == _currentUser.UserId!.Value)
            .Select(u => u.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userOrgId.HasValue && userOrgId.Value == entity.OrganizationId)
        {
            return;
        }

        throw new ForbiddenException("You are not allowed to update this donation request.");
    }

    private async Task EnsurePickupAddressAsync(Guid addressId, Guid donorUserId, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == addressId, cancellationToken);

        if (address is null)
        {
            throw new NotFoundException("Pickup address not found.");
        }

        if (!_currentUser.IsAdmin && address.UserId != donorUserId)
        {
            throw new ForbiddenException("Pickup address must belong to the donor.");
        }
    }

    private async Task<DonationRequestResponse> LoadResponseAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.DonationRequests
            .AsNoTracking()
            .Include(d => d.Photos)
            .Include(d => d.Items)
                .ThenInclude(i => i.Photos)
            .Include(d => d.Items)
                .ThenInclude(i => i.ItemColors)
            .FirstAsync(d => d.Id == id, cancellationToken);

        return DonationRequestMapper.Map(entity);
    }
}
