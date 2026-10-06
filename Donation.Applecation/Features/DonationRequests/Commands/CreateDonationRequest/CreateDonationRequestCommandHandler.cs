using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Application.Features.DonationRequests.Common;
using Donation.Domain.Entities;
using Donation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.DonationRequests.Commands.CreateDonationRequest;

public sealed class CreateDonationRequestCommandHandler
    : IRequestHandler<CreateDonationRequestCommand, DonationRequestResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateDonationRequestCommandHandler> _logger;

    public CreateDonationRequestCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateDonationRequestCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<DonationRequestResponse> Handle(
        CreateDonationRequestCommand request,
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
            var organizationExists = await _context.Organizations
                .AnyAsync(o => o.Id == request.OrganizationId && !o.IsDeleted, cancellationToken);
            if (!organizationExists)
            {
                throw new NotFoundException("Organization not found.");
            }

            var donor = await ResolveDonorAsync(request.DonorId, cancellationToken);

            if (!_currentUser.IsAdmin && donor.UserId != _currentUser.UserId)
            {
                throw new ForbiddenException("You can only create donation requests for your own donor profile.");
            }

            if (request.DeliveryMethod == DeliveryMethod.VolunteerPickup)
            {
                await EnsurePickupAddressAsync(request.PickupAddressId!.Value, donor.UserId, cancellationToken);
            }

            await DonationRequestCatalogGuard.EnsureCatalogReferencesAsync(
                _context, request.Items, cancellationToken);
            await DonationRequestCatalogGuard.EnsureBarcodesUniqueAsync(
                _context,
                request.Items.Select(i => i.Barcode),
                excludeItemIds: null,
                cancellationToken);

            var now = DateTime.UtcNow;
            entity = new DonationRequest
            {
                OrganizationId = request.OrganizationId,
                DonorId = donor.Id,
                PickupAddressId = request.DeliveryMethod == DeliveryMethod.VolunteerPickup
                    ? request.PickupAddressId
                    : null,
                DeliveryMethod = request.DeliveryMethod,
                Description = request.Description.Trim(),
                EstimatedItemCount = request.EstimatedItemCount,
                Status = DonationRequestStatus.Submitted,
                SubmittedAt = now,
                CreatedAt = now,
                IsDeleted = false
            };

            itemPhotos = new Dictionary<Guid, IReadOnlyList<string>>();
            foreach (var itemRequest in request.Items)
            {
                var item = DonationRequestItemFactory.CreateItem(
                    itemRequest,
                    request.OrganizationId,
                    now,
                    _currentUser.UserId);
                entity.Items.Add(item);
                itemPhotos[item.Id] = itemRequest.PhotoUrls;
            }

            _context.DonationRequests.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "DonationRequest created with Id {DonationRequestId} ({ItemCount} items)",
                entity.Id,
                entity.Items.Count);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        await DonationRequestPhotoPersistence.TryPersistCreatePhotosAsync(
            _context,
            entity,
            request.RequestPhotoUrls,
            itemPhotos,
            _logger,
            cancellationToken);

        return await LoadResponseAsync(entity.Id, cancellationToken);
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

    private async Task<Donor> ResolveDonorAsync(Guid? requestedDonorId, CancellationToken cancellationToken)
    {
        if (_currentUser.IsAdmin && requestedDonorId.HasValue && requestedDonorId.Value != Guid.Empty)
        {
            var adminTarget = await _context.Donors
                .FirstOrDefaultAsync(d => d.Id == requestedDonorId.Value, cancellationToken);
            return adminTarget ?? throw new NotFoundException("Donor not found.");
        }

        var own = await _context.Donors
            .FirstOrDefaultAsync(d => d.UserId == _currentUser.UserId!.Value, cancellationToken);
        return own ?? throw new ForbiddenException("Only donors can create donation requests.");
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
}
