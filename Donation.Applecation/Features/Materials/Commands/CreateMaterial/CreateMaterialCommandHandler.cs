using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Material.Response;
using Donation.Application.Features.Materials.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.CreateMaterial;

public sealed class CreateMaterialCommandHandler : IRequestHandler<CreateMaterialCommand, MaterialResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateMaterialCommandHandler> _logger;

    public CreateMaterialCommandHandler(IAppDbContext context, ILogger<CreateMaterialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MaterialResponse> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var nameExists = await _context.Materials
            .AnyAsync(m => m.Name.ToLower() == name.ToLower(), cancellationToken);
        if (nameExists)
        {
            throw new ConflictException("A material with this name already exists.");
        }

        var material = new Material { Name = name, IsDeleted = false };
        _context.Materials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Material created with Id {MaterialId}", material.Id);
        return MaterialMapper.Map(material);
    }
}
