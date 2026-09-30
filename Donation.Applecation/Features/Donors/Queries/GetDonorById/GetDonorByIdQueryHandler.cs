using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Donors.Queries.GetDonorById;

public sealed class GetDonorByIdQueryHandler : IRequestHandler<GetDonorByIdQuery, DonorResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDonorByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DonorResponse?> Handle(GetDonorByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var donor = await _context.Donors
            .AsNoTracking()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (donor is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && donor.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view your own donor profile.");
        }

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
