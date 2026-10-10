using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.ItemPhoto.Response;
using Donation.Application.Features.ItemPhotos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemPhotos.Commands.UpdateItemPhoto;

public sealed class UpdateItemPhotoCommandHandler
    : IRequestHandler<UpdateItemPhotoCommand, ItemPhotoResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateItemPhotoCommandHandler> _logger;

    public UpdateItemPhotoCommandHandler(
        IAppDbContext context,
        ILogger<UpdateItemPhotoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemPhotoResponse?> Handle(
        UpdateItemPhotoCommand request,
        CancellationToken cancellationToken)
    {
        var photo = await _context.ItemPhotos
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (photo is null)
        {
            return null;
        }

        photo.Url = request.Url.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemPhoto updated with Id {ItemPhotoId}", photo.Id);
        return ItemPhotoMapper.Map(photo);
    }
}
