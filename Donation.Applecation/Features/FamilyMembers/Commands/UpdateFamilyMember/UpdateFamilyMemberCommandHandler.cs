using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
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

        var targetBeneficiary = member.Beneficiary;
        if (request.BeneficiaryId != member.BeneficiaryId)
        {
            targetBeneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId && !b.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Beneficiary not found.");

            EnsureCanManageBeneficiary(targetBeneficiary);
        }

        member.BeneficiaryId = request.BeneficiaryId;
        member.FullName = request.FullName.Trim();
        member.BirthDate = request.BirthDate;
        member.Gender = request.Gender;
        member.ClothingSize = request.ClothingSize;
        member.ShoeSize = request.ShoeSize.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FamilyMember updated with Id {FamilyMemberId}", member.Id);

        return new FamilyMemberResponse
        {
            Id = member.Id,
            BeneficiaryId = member.BeneficiaryId,
            FullName = member.FullName,
            BirthDate = member.BirthDate,
            Gender = member.Gender,
            ClothingSize = member.ClothingSize,
            ShoeSize = member.ShoeSize,
            IsDeleted = member.IsDeleted
        };
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
