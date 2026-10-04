using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Material.Response;
using Donation.Application.Features.Materials.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.UpdateMaterial;

public sealed class UpdateMaterialCommandHandler : IRequestHandler<UpdateMaterialCommand, MaterialResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateMaterialCommandHandler> _logger;

    public UpdateMaterialCommandHandler(IAppDbContext context, ILogger<UpdateMaterialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MaterialResponse?> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (material is null)
        {
            return null;
        }

        var name = request.Name.Trim();
        var nameExists = await _context.Materials
            .AnyAsync(m => m.Id != request.Id && m.Name.ToLower() == name.ToLower(), cancellationToken);
        if (nameExists)
        {
            throw new ConflictException("A material with this name already exists.");
        }

        material.Name = name;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Material updated with Id {MaterialId}", material.Id);
        return MaterialMapper.Map(material);
    }
}
