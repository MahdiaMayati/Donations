using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Areas.Commands.DeleteArea;

public sealed class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IAreaDependencyChecker _dependencyChecker;
    private readonly ILogger<DeleteAreaCommandHandler> _logger;

    public DeleteAreaCommandHandler(
        IAppDbContext context,
        IAreaDependencyChecker dependencyChecker,
        ILogger<DeleteAreaCommandHandler> logger)
    {
        _context = context;
        _dependencyChecker = dependencyChecker;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.Areas
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (area is null)
        {
            return false;
        }

        var (hasDependencies, message) = await _dependencyChecker.CheckAsync(request.Id, cancellationToken);
        if (hasDependencies)
        {
            throw new BusinessRuleException(
                string.IsNullOrWhiteSpace(message)
                    ? "Cannot delete this area because it is referenced by related records."
                    : message);
        }

        _context.Areas.Remove(area);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Area deleted with Id {AreaId}", request.Id);

        return true;
    }
}
