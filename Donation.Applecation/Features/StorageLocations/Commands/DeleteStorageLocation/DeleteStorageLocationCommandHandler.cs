using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.StorageLocations.Commands.DeleteStorageLocation;

public sealed class DeleteStorageLocationCommandHandler : IRequestHandler<DeleteStorageLocationCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteStorageLocationCommandHandler> _logger;

    public DeleteStorageLocationCommandHandler(
        IAppDbContext context,
        ILogger<DeleteStorageLocationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteStorageLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.StorageLocations
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (location is null)
        {
            return false;
        }

        _context.StorageLocations.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("StorageLocation deleted with Id {StorageLocationId}", request.Id);

        return true;
    }
}
