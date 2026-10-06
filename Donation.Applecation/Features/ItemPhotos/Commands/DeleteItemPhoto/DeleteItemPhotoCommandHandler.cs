using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemPhotos.Commands.DeleteItemPhoto;

public sealed class DeleteItemPhotoCommandHandler : IRequestHandler<DeleteItemPhotoCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteItemPhotoCommandHandler> _logger;

    public DeleteItemPhotoCommandHandler(
        IAppDbContext context,
        ILogger<DeleteItemPhotoCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemPhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _context.ItemPhotos
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (photo is null)
        {
            return false;
        }

        _context.ItemPhotos.Remove(photo);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemPhoto hard-deleted with Id {ItemPhotoId}", request.Id);
        return true;
    }
}
