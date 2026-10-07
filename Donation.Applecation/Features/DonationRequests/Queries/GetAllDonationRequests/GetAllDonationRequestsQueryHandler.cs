using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.DonationRequest.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.DonationRequests.Queries.GetAllDonationRequests;

public sealed class GetAllDonationRequestsQueryHandler
    : IRequestHandler<GetAllDonationRequestsQuery, PaginatedResult<DonationRequestResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllDonationRequestsQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<DonationRequestResponse>> Handle(
        GetAllDonationRequestsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        if (!_currentUser.IsAdmin)
        {
            var userOrgId = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == _currentUser.UserId.Value)
                .Select(u => u.OrganizationId)
                .FirstOrDefaultAsync(cancellationToken);

            var donorId = await _context.Donors
                .AsNoTracking()
                .Where(d => d.UserId == _currentUser.UserId.Value)
                .Select(d => (Guid?)d.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var isOrgStaff = userOrgId.HasValue && userOrgId.Value == request.OrganizationId;
            var isDonorInOrg = donorId.HasValue;

            if (!isOrgStaff && !isDonorInOrg)
            {
                throw new ForbiddenException("You are not allowed to list donation requests for this organization.");
            }
        }

        var query = _context.DonationRequests
            .AsNoTracking()
            .Where(d => d.OrganizationId == request.OrganizationId);

        if (!_currentUser.IsAdmin)
        {
            var userOrgId = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == _currentUser.UserId!.Value)
                .Select(u => u.OrganizationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!(userOrgId.HasValue && userOrgId.Value == request.OrganizationId))
            {
                var donorId = await _context.Donors
                    .AsNoTracking()
                    .Where(d => d.UserId == _currentUser.UserId!.Value)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                query = query.Where(d => d.DonorId == donorId);
            }
        }

        if (request.DonorId is Guid filterDonor && filterDonor != Guid.Empty)
        {
            query = query.Where(d => d.DonorId == filterDonor);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(d => d.Status == request.Status.Value);
        }

        if (request.DeliveryMethod.HasValue)
        {
            query = query.Where(d => d.DeliveryMethod == request.DeliveryMethod.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(d => d.Description.ToLower().Contains(term));
        }

        // List: request header only (no nested items) for performance.
        return await query
            .OrderByDescending(d => d.SubmittedAt)
            .Select(d => new DonationRequestResponse
            {
                Id = d.Id,
                OrganizationId = d.OrganizationId,
                DonorId = d.DonorId,
                PickupAddressId = d.PickupAddressId,
                DeliveryMethod = d.DeliveryMethod.ToString(),
                Description = d.Description,
                EstimatedItemCount = d.EstimatedItemCount,
                Status = d.Status.ToString(),
                SubmittedAt = d.SubmittedAt,
                IsDeleted = d.IsDeleted,
                DeletedAt = d.DeletedAt,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
