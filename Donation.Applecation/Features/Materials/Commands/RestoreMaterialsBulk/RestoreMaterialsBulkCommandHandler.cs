using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Material.Response;
using Donation.Application.Features.Materials.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.RestoreMaterialsBulk;

public sealed class RestoreMaterialsBulkCommandHandler
    : IRequestHandler<RestoreMaterialsBulkCommand, IReadOnlyList<MaterialResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RestoreMaterialsBulkCommandHandler> _logger;

    public RestoreMaterialsBulkCommandHandler(
        IAppDbContext context,
        ILogger<RestoreMaterialsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MaterialResponse>> Handle(
        RestoreMaterialsBulkCommand request,
        CancellationToken cancellationToken)
    {
        var uniqueIds = request.Ids.Distinct().ToHashSet();

        var materials = await _context.Materials
            .IgnoreQueryFilters()
            .Where(m => uniqueIds.Contains(m.Id) && m.IsDeleted)
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => materials.All(m => m.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Deleted material(s) not found: {string.Join(", ", missing)}.");
        }

        if (materials.GroupBy(m => m.Name.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Cannot restore: duplicate names among selected deleted materials.");
        }

        var names = materials.Select(m => m.Name.ToLower()).ToHashSet();
        var conflicts = await _context.Materials
            .Where(m => names.Contains(m.Name.ToLower()))
            .Select(m => m.Name)
            .ToListAsync(cancellationToken);
        if (conflicts.Count > 0)
        {
            throw new ConflictException(
                $"Cannot restore: active material name(s) already exist: {string.Join(", ", conflicts)}.");
        }

        foreach (var material in materials)
        {
            material.IsDeleted = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk restored {Count} materials", materials.Count);
        return materials.Select(MaterialMapper.Map).ToList();
    }
}
