using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Items.Common;

internal static class ItemCatalogGuard
{
    public static async Task EnsureReferencesAsync(
        IAppDbContext context,
        Guid itemTypeId,
        Guid? materialId,
        Guid? storageLocationId,
        Guid? sortedByUserId,
        CancellationToken cancellationToken)
    {
        var itemTypeExists = await context.ItemTypes
            .AsNoTracking()
            .AnyAsync(t => t.Id == itemTypeId, cancellationToken);
        if (!itemTypeExists)
        {
            throw new NotFoundException("ItemType not found.");
        }

        if (materialId is Guid mid && mid != Guid.Empty)
        {
            var materialExists = await context.Materials
                .AsNoTracking()
                .AnyAsync(m => m.Id == mid, cancellationToken);
            if (!materialExists)
            {
                throw new NotFoundException("Material not found.");
            }
        }

        if (storageLocationId is Guid sid && sid != Guid.Empty)
        {
            var locationExists = await context.StorageLocations
                .AsNoTracking()
                .AnyAsync(s => s.Id == sid, cancellationToken);
            if (!locationExists)
            {
                throw new NotFoundException("StorageLocation not found.");
            }
        }

        if (sortedByUserId is Guid uid && uid != Guid.Empty)
        {
            var userExists = await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == uid, cancellationToken);
            if (!userExists)
            {
                throw new NotFoundException("SortedByUser not found.");
            }
        }
    }

    public static async Task EnsureBarcodeUniqueAsync(
        IAppDbContext context,
        string? barcode,
        Guid? excludeItemId,
        CancellationToken cancellationToken)
    {
        var normalized = NormalizeBarcode(barcode);
        if (normalized is null)
        {
            return;
        }

        var query = context.Items.AsNoTracking()
            .Where(i => i.Barcode != null && i.Barcode.ToLower() == normalized.ToLower());

        if (excludeItemId is Guid id && id != Guid.Empty)
        {
            query = query.Where(i => i.Id != id);
        }

        var exists = await query.AnyAsync(cancellationToken);
        if (exists)
        {
            throw new ConflictException($"Barcode '{normalized}' is already in use.");
        }
    }

    public static string? NormalizeBarcode(string? barcode)
        => string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();

    public static string? NormalizeOptionalText(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
