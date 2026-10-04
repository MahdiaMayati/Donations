using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Material.Response;
using Donation.Application.Features.Materials.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.RestoreMaterial;

public sealed class RestoreMaterialCommandHandler : IRequestHandler<RestoreMaterialCommand, MaterialResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RestoreMaterialCommandHandler> _logger;

    public RestoreMaterialCommandHandler(IAppDbContext context, ILogger<RestoreMaterialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MaterialResponse?> Handle(RestoreMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _context.Materials
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.IsDeleted, cancellationToken);

        if (material is null)
        {
            return null;
        }

        var nameConflict = await _context.Materials
            .AnyAsync(m => m.Name.ToLower() == material.Name.ToLower(), cancellationToken);
        if (nameConflict)
        {
            throw new ConflictException("Cannot restore: an active material with the same name already exists.");
        }

        material.IsDeleted = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Material restored with Id {MaterialId}", request.Id);
        return MaterialMapper.Map(material);
    }
}
