using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Colors.Commands.DeleteColorsBulk;

public sealed class DeleteColorsBulkCommandHandler : IRequestHandler<DeleteColorsBulkCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteColorsBulkCommandHandler> _logger;

    public DeleteColorsBulkCommandHandler(
        IAppDbContext context,
        ILogger<DeleteColorsBulkCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteColorsBulkCommand request, CancellationToken cancellationToken)
    {
        var uniqueIds = request.Ids.Distinct().ToHashSet();

        var colors = await _context.Colors
            .Where(c => uniqueIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => colors.All(c => c.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Color(s) not found: {string.Join(", ", missing)}.");
        }

        foreach (var color in colors)
        {
            color.IsDeleted = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Bulk soft-deleted {Count} colors", colors.Count);
        return colors.Count;
    }
}
