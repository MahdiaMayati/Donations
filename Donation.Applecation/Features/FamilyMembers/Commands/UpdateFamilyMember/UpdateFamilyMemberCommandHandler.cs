using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Application.Features.FamilyMembers.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMember;

public sealed class UpdateFamilyMemberCommandHandler : IRequestHandler<UpdateFamilyMemberCommand, FamilyMemberResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateFamilyMemberCommandHandler> _logger;

    public UpdateFamilyMemberCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateFamilyMemberCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<FamilyMemberResponse?> Handle(UpdateFamilyMemberCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var member = await _context.FamilyMembers
            .Include(m => m.Beneficiary)
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

        if (member is null)
        {
            return null;
        }

        EnsureCanManageBeneficiary(member.Beneficiary);

        var (targetBeneficiary, user) = await ResolveHeadOfHouseholdAsync(request.HeadOfHouseholdId, cancellationToken);
        EnsureCanManageBeneficiary(targetBeneficiary);

        member.BeneficiaryId = targetBeneficiary.Id;
        member.FullName = request.FullName.Trim();
        member.BirthDate = request.DateOfBirth.Date;
        member.Gender = request.Gender;
        member.ClothingSize = request.ClothingSize.Trim();
        member.ShoeSize = request.ShoeSize.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FamilyMember updated with Id {FamilyMemberId}", member.Id);

        return FamilyMemberMapper.Map(member, targetBeneficiary, user);
    }

    private async Task<(Beneficiary Beneficiary, User User)> ResolveHeadOfHouseholdAsync(
        Guid headOfHouseholdId,
        CancellationToken cancellationToken)
    {
        var beneficiary = await _context.Beneficiaries
            .Include(b => b.User)
            .Where(b => b.UserId == headOfHouseholdId && !b.IsDeleted)
            .OrderByDescending(b => b.IsHeadOfHousehold)
            .ThenByDescending(b => b.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (beneficiary is null || beneficiary.User is null)
        {
            throw new NotFoundException("Head of household (beneficiary) not found for the given HeadOfHouseholdId.");
        }

        return (beneficiary, beneficiary.User);
    }

    private void EnsureCanManageBeneficiary(Beneficiary beneficiary)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (beneficiary.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage family members for your own beneficiary profile.");
        }
    }
}
