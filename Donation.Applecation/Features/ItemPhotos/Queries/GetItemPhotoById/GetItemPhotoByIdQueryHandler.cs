using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.ItemPhoto.Response;
using Donation.Application.Features.ItemPhotos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemPhotos.Queries.GetItemPhotoById;

public sealed class GetItemPhotoByIdQueryHandler
    : IRequestHandler<GetItemPhotoByIdQuery, ItemPhotoResponse?>
{
    private readonly IAppDbContext _context;

    public GetItemPhotoByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ItemPhotoResponse?> Handle(
        GetItemPhotoByIdQuery request,
        CancellationToken cancellationToken)
    {
        var photo = await _context.ItemPhotos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return photo is null ? null : ItemPhotoMapper.Map(photo);
    }
}
