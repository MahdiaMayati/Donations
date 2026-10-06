using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Item.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Items.Queries.GetAllItems;

public sealed class GetAllItemsQueryHandler
    : IRequestHandler<GetAllItemsQuery, PaginatedResult<ItemResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllItemsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemResponse>> Handle(
        GetAllItemsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Items.AsNoTracking();

        if (request.OrganizationId is Guid orgId && orgId != Guid.Empty)
        {
            query = query.Where(i => i.OrganizationId == orgId);
        }

        if (request.DonationRequestId is Guid requestId && requestId != Guid.Empty)
        {
            query = query.Where(i => i.DonationRequestId == requestId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(i =>
                (i.Barcode != null && i.Barcode.ToLower().Contains(term))
                || i.Size.ToLower().Contains(term)
                || (i.SorterNotes != null && i.SorterNotes.ToLower().Contains(term)));
        }

        return await query
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new ItemResponse
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                DonationRequestId = i.DonationRequestId,
                ItemTypeId = i.ItemTypeId,
                MaterialId = i.MaterialId,
                StorageLocationId = i.StorageLocationId,
                SortedByUserId = i.SortedByUserId,
                Barcode = i.Barcode,
                TargetGender = i.TargetGender.ToString(),
                AgeGroup = i.AgeGroup.ToString(),
                Size = i.Size,
                Season = i.Season.ToString(),
                Condition = i.Condition.ToString(),
                SortingStatus = i.SortingStatus.ToString(),
                AvailabilityStatus = i.AvailabilityStatus.ToString(),
                SorterNotes = i.SorterNotes,
                ReceivedAt = i.ReceivedAt,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                IsDeleted = i.IsDeleted,
                DeletedAt = i.DeletedAt,
                ColorIds = i.ItemColors.Select(ic => ic.ColorId).ToList(),
                Photos = i.Photos
                    .OrderBy(p => p.CreatedAt)
                    .Select(p => new ItemPhotoSummaryResponse
                    {
                        Id = p.Id,
                        Url = p.Url,
                        CreatedAt = p.CreatedAt
                    })
                    .ToList()
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
