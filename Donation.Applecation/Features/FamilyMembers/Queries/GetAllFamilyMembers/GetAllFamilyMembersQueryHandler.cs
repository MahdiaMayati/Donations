using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.FamilyMembers.Queries.GetAllFamilyMembers;

public sealed class GetAllFamilyMembersQueryHandler : IRequestHandler<GetAllFamilyMembersQuery, IReadOnlyList<FamilyMemberResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllFamilyMembersQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<FamilyMemberResponse>> Handle(GetAllFamilyMembersQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var query = _context.FamilyMembers
            .AsNoTracking()
            .Where(m => !m.IsDeleted);

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(m => m.Beneficiary.UserId == _currentUser.UserId.Value);
        }

        if (request.BeneficiaryId.HasValue)
        {
            query = query.Where(m => m.BeneficiaryId == request.BeneficiaryId.Value);
        }

        return await query
            .OrderByDescending(m => m.Id)
            .Select(m => new FamilyMemberResponse
            {
                Id = m.Id,
                BeneficiaryId = m.BeneficiaryId,
                FullName = m.FullName,
                BirthDate = m.BirthDate,
                Gender = m.Gender,
                ClothingSize = m.ClothingSize,
                ShoeSize = m.ShoeSize,
                IsDeleted = m.IsDeleted
            })
            .ToListAsync(cancellationToken);
    }
}
