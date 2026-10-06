using Donation.Application.Abstractions.Persistence;
using Donation.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.DonationRequests.Common;

/// <summary>
/// Persists Cloudinary photo URLs after the core aggregate commit.
/// Failures are logged and never roll back request/items/colors/history.
/// </summary>
internal static class DonationRequestPhotoPersistence
{
    public static async Task TryPersistCreatePhotosAsync(
        IAppDbContext context,
        DonationRequest request,
        IReadOnlyList<string> requestPhotoUrls,
        IReadOnlyDictionary<Guid, IReadOnlyList<string>> itemPhotoUrlsByItemId,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;

            foreach (var url in NormalizeUrls(requestPhotoUrls))
            {
                context.DonationRequestPhotos.Add(new DonationRequestPhoto
                {
                    DonationRequestId = request.Id,
                    Url = url,
                    CreatedAt = now
                });
            }

            foreach (var (itemId, urls) in itemPhotoUrlsByItemId)
            {
                foreach (var url in NormalizeUrls(urls))
                {
                    context.ItemPhotos.Add(new ItemPhoto
                    {
                        ItemId = itemId,
                        Url = url,
                        CreatedAt = now
                    });
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Best-effort photo persistence failed for DonationRequest {DonationRequestId}. Core aggregate was already committed.",
                request.Id);
        }
    }

    public static async Task TrySyncUpdatePhotosAsync(
        IAppDbContext context,
        DonationRequest request,
        IReadOnlyList<string> requestPhotoUrls,
        IReadOnlyDictionary<Guid, IReadOnlyList<string>> itemPhotoUrlsByItemId,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            SyncRequestPhotos(request, requestPhotoUrls, now);

            foreach (var item in request.Items.Where(i => !i.IsDeleted))
            {
                if (!itemPhotoUrlsByItemId.TryGetValue(item.Id, out var urls))
                {
                    continue;
                }

                SyncItemPhotos(item, urls, now);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Best-effort photo sync failed for DonationRequest {DonationRequestId}. Core aggregate update was already committed.",
                request.Id);
        }
    }

    private static void SyncRequestPhotos(
        DonationRequest request,
        IReadOnlyList<string> desiredUrls,
        DateTime now)
    {
        var desired = NormalizeUrls(desiredUrls).ToHashSet(StringComparer.Ordinal);
        var existing = request.Photos.ToList();

        foreach (var photo in existing.Where(p => !desired.Contains(p.Url)))
        {
            request.Photos.Remove(photo);
        }

        var present = request.Photos.Select(p => p.Url).ToHashSet(StringComparer.Ordinal);
        foreach (var url in desired.Where(u => !present.Contains(u)))
        {
            request.Photos.Add(new DonationRequestPhoto
            {
                Url = url,
                CreatedAt = now
            });
        }
    }

    private static void SyncItemPhotos(Item item, IReadOnlyList<string> desiredUrls, DateTime now)
    {
        var desired = NormalizeUrls(desiredUrls).ToHashSet(StringComparer.Ordinal);
        var existing = item.Photos.ToList();

        foreach (var photo in existing.Where(p => !desired.Contains(p.Url)))
        {
            item.Photos.Remove(photo);
        }

        var present = item.Photos.Select(p => p.Url).ToHashSet(StringComparer.Ordinal);
        foreach (var url in desired.Where(u => !present.Contains(u)))
        {
            item.Photos.Add(new ItemPhoto
            {
                Url = url,
                CreatedAt = now
            });
        }
    }

    private static IEnumerable<string> NormalizeUrls(IEnumerable<string> urls)
        => urls
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct(StringComparer.Ordinal);
}
