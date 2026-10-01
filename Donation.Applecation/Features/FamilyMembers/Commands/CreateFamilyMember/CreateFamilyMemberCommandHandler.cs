using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Application.Features.FamilyMembers.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMember;

public sealed class CreateFamilyMemberCommandHandler : IRequestHandler<CreateFamilyMemberCommand, FamilyMemberResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateFamilyMemberCommandHandler> _logger;

    public CreateFamilyMemberCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateFamilyMemberCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<FamilyMemberResponse> Handle(CreateFamilyMemberCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var (beneficiary, user) = await ResolveHeadOfHouseholdAsync(request.HeadOfHouseholdId, cancellationToken);
        EnsureCanManageBeneficiary(beneficiary);

        var member = new FamilyMember
        {
            BeneficiaryId = beneficiary.Id,
            FullName = request.FullName.Trim(),
            BirthDate = request.DateOfBirth.Date,
            Gender = request.Gender,
            ClothingSize = request.ClothingSize.Trim(),
            ShoeSize = request.ShoeSize.Trim(),
            IsDeleted = false
        };

        _context.FamilyMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FamilyMember created with Id {FamilyMemberId}", member.Id);

        return FamilyMemberMapper.Map(member, beneficiary, user);
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
