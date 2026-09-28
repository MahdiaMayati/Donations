using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Cities.Commands.DeleteCity;

public sealed class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICityDependencyChecker _dependencyChecker;
    private readonly ILogger<DeleteCityCommandHandler> _logger;

    public DeleteCityCommandHandler(
        IAppDbContext context,
        ICityDependencyChecker dependencyChecker,
        ILogger<DeleteCityCommandHandler> logger)
    {
        _context = context;
        _dependencyChecker = dependencyChecker;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (city is null)
        {
            return false;
        }

        var (hasDependencies, message) = await _dependencyChecker.CheckAsync(request.Id, cancellationToken);
        if (hasDependencies)
        {
            throw new BusinessRuleException(
                string.IsNullOrWhiteSpace(message)
                    ? "Cannot delete this city because it is referenced by related records."
                    : message);
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("City deleted with Id {CityId}", request.Id);

        return true;
    }
}
