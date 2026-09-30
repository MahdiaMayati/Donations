using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Volunteers.Queries.GetDeletedVolunteers;

public sealed class GetDeletedVolunteersQueryHandler
    : IRequestHandler<GetDeletedVolunteersQuery, IReadOnlyList<VolunteerResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeletedVolunteersQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<VolunteerResponse>> Handle(
        GetDeletedVolunteersQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Only Admin users can view soft-deleted volunteers.");
        }

        var volunteers = await _context.Volunteers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(v => v.User)
            .Include(v => v.Organization)
            .Where(v => v.IsDeleted)
            .OrderByDescending(v => v.DeletedAt)
            .ThenByDescending(v => v.Id)
            .ToListAsync(cancellationToken);

        var userIds = volunteers.Select(v => v.UserId).Distinct().ToList();

        var addresses = await _context.Addresses
            .AsNoTracking()
            .Include(a => a.Area)
                .ThenInclude(ar => ar.City)
            .Where(a => userIds.Contains(a.UserId))
            .ToListAsync(cancellationToken);

        var addressByUser = addresses
            .GroupBy(a => a.UserId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Id).First());

        return volunteers
            .Select(v => VolunteerMapping.ToResponse(
                v,
                v.User,
                v.Organization,
                addressByUser.GetValueOrDefault(v.UserId)))
            .ToList();
    }
}
