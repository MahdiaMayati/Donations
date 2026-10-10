using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Application.Features.FamilyMembers.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.FamilyMembers.Queries.GetAllFamilyMembers;

public sealed class GetAllFamilyMembersQueryHandler
    : IRequestHandler<GetAllFamilyMembersQuery, PaginatedResult<FamilyMemberResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllFamilyMembersQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<FamilyMemberResponse>> Handle(
        GetAllFamilyMembersQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var query = _context.FamilyMembers
            .AsNoTracking()
            .Include(m => m.Beneficiary)
                .ThenInclude(b => b.User)
            .Where(m => !m.IsDeleted);

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(m => m.Beneficiary.UserId == _currentUser.UserId.Value);
        }

        if (request.HeadOfHouseholdId.HasValue)
        {
            query = query.Where(m => m.Beneficiary.UserId == request.HeadOfHouseholdId.Value);
        }

        if (request.Ids is { Count: > 0 })
        {
            var ids = request.Ids.ToHashSet();
            query = query.Where(m => ids.Contains(m.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(m => m.FullName.ToLower().Contains(term));
        }

        var page = request.Page < 1 ? PaginationRequest.DefaultPage : request.Page;
        var limit = request.Limit < 1
            ? PaginationRequest.DefaultLimit
            : Math.Min(request.Limit, PaginationRequest.MaxLimit);

        var totalItems = await query.CountAsync(cancellationToken);

        var members = await query
            .OrderByDescending(m => m.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = members
            .Select(m => FamilyMemberMapper.Map(m, m.Beneficiary, m.Beneficiary.User))
            .ToList();

        return new PaginatedResult<FamilyMemberResponse>
        {
            Items = items,
            Pagination = PaginationMetadata.Create(page, limit, totalItems)
        };
    }
}
