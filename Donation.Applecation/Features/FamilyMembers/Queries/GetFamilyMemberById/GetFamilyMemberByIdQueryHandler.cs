using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Application.Features.FamilyMembers.Common;
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
            .Include(m => m.Beneficiary)
                .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

        if (member is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && member.Beneficiary.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view family members for your own beneficiary profile.");
        }

        return FamilyMemberMapper.Map(member, member.Beneficiary, member.Beneficiary.User);
    }
}
