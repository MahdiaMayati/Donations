using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.DeleteMaterialsBulk;

public sealed class DeleteMaterialsBulkCommandHandler : IRequestHandler<DeleteMaterialsBulkCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteMaterialsBulkCommandHandler> _logger;

    public DeleteMaterialsBulkCommandHandler(
        IAppDbContext context,
        ILogger<DeleteMaterialsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteMaterialsBulkCommand request, CancellationToken cancellationToken)
    {
        var uniqueIds = request.Ids.Distinct().ToHashSet();

        var materials = await _context.Materials
            .Where(m => uniqueIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => materials.All(m => m.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Material(s) not found: {string.Join(", ", missing)}.");
        }

        foreach (var material in materials)
        {
            material.IsDeleted = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk soft-deleted {Count} materials", materials.Count);
        return materials.Count;
    }
}
