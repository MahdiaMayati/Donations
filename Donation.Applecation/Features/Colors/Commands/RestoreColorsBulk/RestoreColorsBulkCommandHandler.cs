using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Color.Response;
using Donation.Application.Features.Colors.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.RestoreColorsBulk;

public sealed class RestoreColorsBulkCommandHandler
    : IRequestHandler<RestoreColorsBulkCommand, IReadOnlyList<ColorResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RestoreColorsBulkCommandHandler> _logger;

    public RestoreColorsBulkCommandHandler(
        IAppDbContext context,
        ILogger<RestoreColorsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ColorResponse>> Handle(
        RestoreColorsBulkCommand request,
        CancellationToken cancellationToken)
    {
        var uniqueIds = request.Ids.Distinct().ToHashSet();

        var colors = await _context.Colors
            .IgnoreQueryFilters()
            .Where(c => uniqueIds.Contains(c.Id) && c.IsDeleted)
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => colors.All(c => c.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Deleted color(s) not found: {string.Join(", ", missing)}.");
        }

        if (colors.GroupBy(c => c.Name.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Cannot restore: duplicate names among selected deleted colors.");
        }

        if (colors.GroupBy(c => c.Code.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Cannot restore: duplicate codes among selected deleted colors.");
        }

        var names = colors.Select(c => c.Name.ToLower()).ToHashSet();
        var codes = colors.Select(c => c.Code.ToLower()).ToHashSet();

        var nameConflicts = await _context.Colors
            .Where(c => names.Contains(c.Name.ToLower()))
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);
        if (nameConflicts.Count > 0)
        {
            throw new ConflictException(
                $"Cannot restore: active color name(s) already exist: {string.Join(", ", nameConflicts)}.");
        }

        var codeConflicts = await _context.Colors
            .Where(c => codes.Contains(c.Code.ToLower()))
            .Select(c => c.Code)
            .ToListAsync(cancellationToken);
        if (codeConflicts.Count > 0)
        {
            throw new ConflictException(
                $"Cannot restore: active color code(s) already exist: {string.Join(", ", codeConflicts)}.");
        }

        foreach (var color in colors)
        {
            color.IsDeleted = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk restored {Count} colors", colors.Count);

        return colors.Select(ColorMapper.Map).ToList();
    }
}
