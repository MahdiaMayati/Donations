using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemPhoto.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemPhotos.Queries.GetAllItemPhotos;

public sealed class GetAllItemPhotosQueryHandler
    : IRequestHandler<GetAllItemPhotosQuery, PaginatedResult<ItemPhotoResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllItemPhotosQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemPhotoResponse>> Handle(
        GetAllItemPhotosQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ItemPhotos.AsNoTracking();

        if (request.ItemId is Guid itemId && itemId != Guid.Empty)
        {
            query = query.Where(p => p.ItemId == itemId);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ItemPhotoResponse
            {
                Id = p.Id,
                ItemId = p.ItemId,
                Url = p.Url,
                CreatedAt = p.CreatedAt
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
