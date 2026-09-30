using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Donors.Commands.RestoreDonor;

public sealed class RestoreDonorCommandHandler : IRequestHandler<RestoreDonorCommand, DonorResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<RestoreDonorCommandHandler> _logger;

    public RestoreDonorCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<RestoreDonorCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<DonorResponse?> Handle(RestoreDonorCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Only Admin users can restore soft-deleted donors.");
        }

        var donor = await _context.Donors
            .IgnoreQueryFilters()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (donor is null || !donor.IsDeleted)
        {
            return null;
        }

        var activeExists = await _context.Donors
            .AnyAsync(d => d.UserId == donor.UserId && !d.IsDeleted, cancellationToken);

        if (activeExists)
        {
            throw new ConflictException(
                "Cannot restore this donor because an active donor profile already exists for the same user.");
        }

        donor.IsDeleted = false;
        donor.DeletedAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Donor restored with Id {DonorId}", donor.Id);

        var address = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .Where(a => a.UserId == donor.UserId)
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return DonorMapping.ToResponse(donor, donor.User, address);
    }
}
