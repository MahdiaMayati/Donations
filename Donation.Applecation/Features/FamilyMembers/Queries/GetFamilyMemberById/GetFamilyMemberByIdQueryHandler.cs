using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.FamilyMembers.Queries.GetFamilyMemberById;

public sealed class GetFamilyMemberByIdQueryHandler : IRequestHandler<GetFamilyMemberByIdQuery, FamilyMemberResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetFamilyMemberByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<FamilyMemberResponse?> Handle(GetFamilyMemberByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var member = await _context.FamilyMembers
            .AsNoTracking()
            .Where(m => m.Id == request.Id && !m.IsDeleted)
            .Select(m => new
            {
                Response = new FamilyMemberResponse
                {
                    Id = m.Id,
                    BeneficiaryId = m.BeneficiaryId,
                    FullName = m.FullName,
                    BirthDate = m.BirthDate,
                    Gender = m.Gender,
                    ClothingSize = m.ClothingSize,
                    ShoeSize = m.ShoeSize,
                    IsDeleted = m.IsDeleted
                },
                OwnerUserId = m.Beneficiary.UserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (member is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && member.OwnerUserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view family members for your own beneficiary profile.");
        }

        return member.Response;
    }
}
