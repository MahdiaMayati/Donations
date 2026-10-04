using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.DeleteMaterial;

public sealed class DeleteMaterialCommandHandler : IRequestHandler<DeleteMaterialCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteMaterialCommandHandler> _logger;

    public DeleteMaterialCommandHandler(IAppDbContext context, ILogger<DeleteMaterialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (material is null)
        {
            return false;
        }

        material.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Material soft-deleted with Id {MaterialId}", request.Id);
        return true;
    }
}
