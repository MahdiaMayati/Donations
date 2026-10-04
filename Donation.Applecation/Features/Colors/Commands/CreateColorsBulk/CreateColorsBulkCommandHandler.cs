using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Color.Response;
using Donation.Application.Features.Colors.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.CreateColorsBulk;

public sealed class CreateColorsBulkCommandHandler
    : IRequestHandler<CreateColorsBulkCommand, IReadOnlyList<ColorResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateColorsBulkCommandHandler> _logger;

    public CreateColorsBulkCommandHandler(
        IAppDbContext context,
        ILogger<CreateColorsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ColorResponse>> Handle(
        CreateColorsBulkCommand request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Items
            .Select(i => (Name: i.Name.Trim(), Code: i.Code.Trim()))
            .ToList();

        var duplicateNamesInBatch = normalized
            .GroupBy(i => i.Name.ToLower())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateNamesInBatch.Count > 0)
        {
            throw new ConflictException("Duplicate color names in the batch request.");
        }

        var duplicateCodesInBatch = normalized
            .GroupBy(i => i.Code.ToLower())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateCodesInBatch.Count > 0)
        {
            throw new ConflictException("Duplicate color codes in the batch request.");
        }

        var names = normalized.Select(i => i.Name.ToLower()).ToHashSet();
        var codes = normalized.Select(i => i.Code.ToLower()).ToHashSet();

        var existingNames = await _context.Colors
            .Where(c => names.Contains(c.Name.ToLower()))
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);
        if (existingNames.Count > 0)
        {
            throw new ConflictException($"Color name(s) already exist: {string.Join(", ", existingNames)}.");
        }

        var existingCodes = await _context.Colors
            .Where(c => codes.Contains(c.Code.ToLower()))
            .Select(c => c.Code)
            .ToListAsync(cancellationToken);
        if (existingCodes.Count > 0)
        {
            throw new ConflictException($"Color code(s) already exist: {string.Join(", ", existingCodes)}.");
        }

        var created = new List<Color>();
        foreach (var item in normalized)
        {
            var color = new Color
            {
                Name = item.Name,
                Code = item.Code,
                IsDeleted = false
            };
            _context.Colors.Add(color);
            created.Add(color);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk created {Count} colors", created.Count);

        return created.Select(ColorMapper.Map).ToList();
    }
}
