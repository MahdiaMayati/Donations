using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Color.Response;
using Donation.Application.Features.Colors.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.UpdateColorsBulk;

public sealed class UpdateColorsBulkCommandHandler
    : IRequestHandler<UpdateColorsBulkCommand, IReadOnlyList<ColorResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateColorsBulkCommandHandler> _logger;

    public UpdateColorsBulkCommandHandler(
        IAppDbContext context,
        ILogger<UpdateColorsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ColorResponse>> Handle(
        UpdateColorsBulkCommand request,
        CancellationToken cancellationToken)
    {
        var uniqueIds = request.Items.Select(i => i.Id).Distinct().ToHashSet();
        if (uniqueIds.Count != request.Items.Count)
        {
            throw new ConflictException("Duplicate color Ids in the batch request.");
        }

        var colors = await _context.Colors
            .Where(c => uniqueIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => colors.All(c => c.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Color(s) not found: {string.Join(", ", missing)}.");
        }

        var byId = colors.ToDictionary(c => c.Id);
        var pending = request.Items
            .Select(i => (i.Id, Name: i.Name.Trim(), Code: i.Code.Trim()))
            .ToList();

        if (pending.GroupBy(p => p.Name.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Duplicate color names in the batch request.");
        }

        if (pending.GroupBy(p => p.Code.ToLower()).Any(g => g.Count() > 1))
        {
            throw new ConflictException("Duplicate color codes in the batch request.");
        }

        var names = pending.Select(p => p.Name.ToLower()).ToHashSet();
        var codes = pending.Select(p => p.Code.ToLower()).ToHashSet();

        var conflictingNames = await _context.Colors
            .Where(c => !uniqueIds.Contains(c.Id) && names.Contains(c.Name.ToLower()))
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);
        if (conflictingNames.Count > 0)
        {
            throw new ConflictException($"Color name(s) already exist: {string.Join(", ", conflictingNames)}.");
        }

        var conflictingCodes = await _context.Colors
            .Where(c => !uniqueIds.Contains(c.Id) && codes.Contains(c.Code.ToLower()))
            .Select(c => c.Code)
            .ToListAsync(cancellationToken);
        if (conflictingCodes.Count > 0)
        {
            throw new ConflictException($"Color code(s) already exist: {string.Join(", ", conflictingCodes)}.");
        }

        foreach (var item in pending)
        {
            var color = byId[item.Id];
            color.Name = item.Name;
            color.Code = item.Code;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk updated {Count} colors", pending.Count);

        return pending.Select(p => ColorMapper.Map(byId[p.Id])).ToList();
    }
}
