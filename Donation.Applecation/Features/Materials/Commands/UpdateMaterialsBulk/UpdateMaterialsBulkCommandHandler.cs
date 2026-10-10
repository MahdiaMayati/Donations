using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Material.Response;
using Donation.Application.Features.Materials.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.UpdateMaterialsBulk;

public sealed class UpdateMaterialsBulkCommandHandler
    : IRequestHandler<UpdateMaterialsBulkCommand, IReadOnlyList<MaterialResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateMaterialsBulkCommandHandler> _logger;

    public UpdateMaterialsBulkCommandHandler(
        IAppDbContext context,
        ILogger<UpdateMaterialsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MaterialResponse>> Handle(
        UpdateMaterialsBulkCommand request,
        CancellationToken cancellationToken)
    {
        var uniqueIds = request.Items.Select(i => i.Id).Distinct().ToHashSet();
        if (uniqueIds.Count != request.Items.Count)
        {
            throw new ConflictException("Duplicate material Ids in the batch request.");
        }

        var materials = await _context.Materials
            .Where(m => uniqueIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => materials.All(m => m.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Material(s) not found: {string.Join(", ", missing)}.");
        }

        var byId = materials.ToDictionary(m => m.Id);
        var pending = request.Items.Select(i => (i.Id, Name: i.Name.Trim())).ToList();

        if (pending.GroupBy(p => p.Name.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Duplicate material names in the batch request.");
        }

        var names = pending.Select(p => p.Name.ToLower()).ToHashSet();
        var conflicts = await _context.Materials
            .Where(m => !uniqueIds.Contains(m.Id) && names.Contains(m.Name.ToLower()))
            .Select(m => m.Name)
            .ToListAsync(cancellationToken);
        if (conflicts.Count > 0)
        {
            throw new ConflictException($"Material name(s) already exist: {string.Join(", ", conflicts)}.");
        }

        foreach (var item in pending)
        {
            byId[item.Id].Name = item.Name;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk updated {Count} materials", pending.Count);
        return pending.Select(p => MaterialMapper.Map(byId[p.Id])).ToList();
    }
}
