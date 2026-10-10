using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Material.Response;
using Donation.Application.Features.Materials.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Materials.Commands.CreateMaterialsBulk;

public sealed class CreateMaterialsBulkCommandHandler
    : IRequestHandler<CreateMaterialsBulkCommand, IReadOnlyList<MaterialResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateMaterialsBulkCommandHandler> _logger;

    public CreateMaterialsBulkCommandHandler(
        IAppDbContext context,
        ILogger<CreateMaterialsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MaterialResponse>> Handle(
        CreateMaterialsBulkCommand request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Items.Select(i => i.Name.Trim()).ToList();

        if (normalized.GroupBy(n => n.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Duplicate material names in the batch request.");
        }

        var names = normalized.Select(n => n.ToLower()).ToHashSet();
        var existing = await _context.Materials
            .Where(m => names.Contains(m.Name.ToLower()))
            .Select(m => m.Name)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            throw new ConflictException($"Material name(s) already exist: {string.Join(", ", existing)}.");
        }

        var created = new List<Material>();
        foreach (var name in normalized)
        {
            var material = new Material { Name = name, IsDeleted = false };
            _context.Materials.Add(material);
            created.Add(material);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk created {Count} materials", created.Count);
        return created.Select(MaterialMapper.Map).ToList();
    }
}
