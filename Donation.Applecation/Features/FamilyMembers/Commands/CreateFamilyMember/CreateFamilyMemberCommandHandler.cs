using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
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

        var beneficiary = await _context.Beneficiaries
            .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId && !b.IsDeleted, cancellationToken);

        if (beneficiary is null)
        {
            throw new NotFoundException("Beneficiary not found.");
        }

        EnsureCanManageBeneficiary(beneficiary);

        var member = new FamilyMember
        {
            BeneficiaryId = request.BeneficiaryId,
            FullName = request.FullName.Trim(),
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            ClothingSize = request.ClothingSize,
            ShoeSize = request.ShoeSize.Trim(),
            IsDeleted = false
        };

        _context.FamilyMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FamilyMember created with Id {FamilyMemberId}", member.Id);

        return Map(member);
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

    private static FamilyMemberResponse Map(FamilyMember m) => new()
    {
        Id = m.Id,
        BeneficiaryId = m.BeneficiaryId,
        FullName = m.FullName,
        BirthDate = m.BirthDate,
        Gender = m.Gender,
        ClothingSize = m.ClothingSize,
        ShoeSize = m.ShoeSize,
        IsDeleted = m.IsDeleted
    };
}
