using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Volunteers.Queries.GetDeletedVolunteersCount;

public sealed class GetDeletedVolunteersCountQueryHandler : IRequestHandler<GetDeletedVolunteersCountQuery, int>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeletedVolunteersCountQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(GetDeletedVolunteersCountQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Only Admin users can view soft-deleted volunteer counts.");
        }

        return await _context.Volunteers
            .IgnoreQueryFilters()
            .CountAsync(v => v.IsDeleted, cancellationToken);
    }
}
