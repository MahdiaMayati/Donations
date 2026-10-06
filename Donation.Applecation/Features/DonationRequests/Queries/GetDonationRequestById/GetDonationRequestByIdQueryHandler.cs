using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Application.Features.DonationRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.DonationRequests.Queries.GetDonationRequestById;

public sealed class GetDonationRequestByIdQueryHandler
    : IRequestHandler<GetDonationRequestByIdQuery, DonationRequestResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDonationRequestByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DonationRequestResponse?> Handle(
        GetDonationRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var entity = await _context.DonationRequests
            .AsNoTracking()
            .Include(d => d.Donor)
            .Include(d => d.Photos)
            .Include(d => d.Items)
                .ThenInclude(i => i.Photos)
            .Include(d => d.Items)
                .ThenInclude(i => i.ItemColors)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin)
        {
            var isOwner = entity.Donor.UserId == _currentUser.UserId;
            var userOrgId = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == _currentUser.UserId.Value)
                .Select(u => u.OrganizationId)
                .FirstOrDefaultAsync(cancellationToken);

            var isOrgStaff = userOrgId.HasValue && userOrgId.Value == entity.OrganizationId;
            if (!isOwner && !isOrgStaff)
            {
                throw new ForbiddenException("You are not allowed to view this donation request.");
            }
        }

        return DonationRequestMapper.Map(entity);
    }
}
