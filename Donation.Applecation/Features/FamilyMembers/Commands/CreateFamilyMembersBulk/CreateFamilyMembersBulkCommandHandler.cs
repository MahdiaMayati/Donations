using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Application.Features.FamilyMembers.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMembersBulk;

public sealed class CreateFamilyMembersBulkCommandHandler
    : IRequestHandler<CreateFamilyMembersBulkCommand, IReadOnlyList<FamilyMemberResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateFamilyMembersBulkCommandHandler> _logger;

    public CreateFamilyMembersBulkCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateFamilyMembersBulkCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FamilyMemberResponse>> Handle(
        CreateFamilyMembersBulkCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        // تم التعديل هنا لاستخدام ToHashSet لتجنب مشاكل الترجمة مع SQL Server
        var headIds = request.Items.Select(i => i.HeadOfHouseholdId).Distinct().ToHashSet();

        var beneficiaries = await _context.Beneficiaries
            .Include(b => b.User)
            .Where(b => headIds.Contains(b.UserId) && !b.IsDeleted)
            .ToListAsync(cancellationToken);

        // Prefer IsHeadOfHousehold when multiple beneficiary rows exist per user
        var beneficiaryByUserId = beneficiaries
            .GroupBy(b => b.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(b => b.IsHeadOfHousehold).ThenByDescending(b => b.Id).First());

        var missing = headIds.Where(id => !beneficiaryByUserId.ContainsKey(id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException(
                $"Head of household (beneficiary) not found for HeadOfHouseholdId(s): {string.Join(", ", missing)}.");
        }

        foreach (var beneficiary in beneficiaryByUserId.Values)
        {
            EnsureCanManageBeneficiary(beneficiary);
        }

        var created = new List<(FamilyMember Member, Beneficiary Beneficiary, User User)>();

        foreach (var item in request.Items)
        {
            var beneficiary = beneficiaryByUserId[item.HeadOfHouseholdId];
            var member = new FamilyMember
            {
                BeneficiaryId = beneficiary.Id,
                FullName = item.FullName.Trim(),
                BirthDate = item.DateOfBirth.Date,
                Gender = item.Gender,
                ClothingSize = item.ClothingSize.Trim(),
                ShoeSize = item.ShoeSize.Trim(),
                IsDeleted = false
            };

            _context.FamilyMembers.Add(member);
            created.Add((member, beneficiary, beneficiary.User));
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bulk created {Count} family members", created.Count);

        return created
            .Select(c => FamilyMemberMapper.Map(c.Member, c.Beneficiary, c.User))
            .ToList();
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