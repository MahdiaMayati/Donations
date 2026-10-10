using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.DonationRequest.Request;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.DonationRequests.Common;

internal static class DonationRequestCatalogGuard
{
    public static async Task EnsureCatalogReferencesAsync(
        IAppDbContext context,
        IReadOnlyList<DonationRequestItemRequest> items,
        CancellationToken cancellationToken)
    {
        var itemTypeIds = items.Select(i => i.ItemTypeId).Distinct().ToList();
        var materialIds = items
            .Where(i => i.MaterialId.HasValue && i.MaterialId.Value != Guid.Empty)
            .Select(i => i.MaterialId!.Value)
            .Distinct()
            .ToList();
        var colorIds = items.SelectMany(i => i.ColorIds).Distinct().ToList();

        var existingItemTypeIds = await context.ItemTypes
            .AsNoTracking()
            .Where(t => EF.Constant(itemTypeIds).Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        var missingItemType = itemTypeIds.FirstOrDefault(id => !existingItemTypeIds.Contains(id));
        if (missingItemType != Guid.Empty)
        {
            throw new NotFoundException($"ItemType '{missingItemType}' not found.");
        }

        if (materialIds.Count > 0)
        {
            var existingMaterialIds = await context.Materials
                .AsNoTracking()
                .Where(m => EF.Constant(materialIds).Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync(cancellationToken);

            var missingMaterial = materialIds.FirstOrDefault(id => !existingMaterialIds.Contains(id));
            if (missingMaterial != Guid.Empty)
            {
                throw new NotFoundException($"Material '{missingMaterial}' not found.");
            }
        }

        var existingColorIds = await context.Colors
            .AsNoTracking()
            .Where(c => EF.Constant(colorIds).Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var missingColor = colorIds.FirstOrDefault(id => !existingColorIds.Contains(id));
        if (missingColor != Guid.Empty)
        {
            throw new NotFoundException($"Color '{missingColor}' not found.");
        }
    }

    public static async Task EnsureBarcodesUniqueAsync(
        IAppDbContext context,
        IEnumerable<string?> barcodes,
        IEnumerable<Guid>? excludeItemIds,
        CancellationToken cancellationToken)
    {
        var normalized = barcodes
            .Where(b => !string.IsNullOrWhiteSpace(b))
            .Select(b => b!.Trim())
            .ToList();

        if (normalized.Count == 0)
        {
            return;
        }

        var duplicatesInPayload = normalized
            .GroupBy(b => b, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .FirstOrDefault();

        if (duplicatesInPayload is not null)
        {
            throw new ConflictException($"Duplicate barcode '{duplicatesInPayload}' in the request.");
        }

        var exclude = excludeItemIds?.Where(id => id != Guid.Empty).Distinct().ToList() ?? new List<Guid>();
        var clash = await context.Items
            .AsNoTracking()
            .Where(i => i.Barcode != null && EF.Constant(normalized).Contains(i.Barcode))
            .Where(i => exclude.Count == 0 || !EF.Constant(exclude).Contains(i.Id))
            .Select(i => i.Barcode)
            .FirstOrDefaultAsync(cancellationToken);

        if (clash is not null)
        {
            throw new ConflictException($"Barcode '{clash}' is already in use.");
        }
    }
}
