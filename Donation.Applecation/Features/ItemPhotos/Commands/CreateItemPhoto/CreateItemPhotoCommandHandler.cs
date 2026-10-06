using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemPhoto.Response;
using Donation.Application.Features.ItemPhotos.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemPhotos.Commands.CreateItemPhoto;

public sealed class CreateItemPhotoCommandHandler
    : IRequestHandler<CreateItemPhotoCommand, ItemPhotoResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateItemPhotoCommandHandler> _logger;

    public CreateItemPhotoCommandHandler(
        IAppDbContext context,
        ILogger<CreateItemPhotoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemPhotoResponse> Handle(
        CreateItemPhotoCommand request,
        CancellationToken cancellationToken)
    {
        var itemExists = await _context.Items
            .AsNoTracking()
            .AnyAsync(i => i.Id == request.ItemId, cancellationToken);
        if (!itemExists)
        {
            throw new NotFoundException("Item not found.");
        }

        var photo = new ItemPhoto
        {
            ItemId = request.ItemId,
            Url = request.Url.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.ItemPhotos.Add(photo);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemPhoto created with Id {ItemPhotoId}", photo.Id);
        return ItemPhotoMapper.Map(photo);
    }
}
