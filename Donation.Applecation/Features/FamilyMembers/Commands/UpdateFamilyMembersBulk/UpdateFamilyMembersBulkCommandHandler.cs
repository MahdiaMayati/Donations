using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Application.Features.FamilyMembers.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMembersBulk;

public sealed class UpdateFamilyMembersBulkCommandHandler
    : IRequestHandler<UpdateFamilyMembersBulkCommand, IReadOnlyList<FamilyMemberResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateFamilyMembersBulkCommandHandler> _logger;

    public UpdateFamilyMembersBulkCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateFamilyMembersBulkCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FamilyMemberResponse>> Handle(
        UpdateFamilyMembersBulkCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        // تم التعديل إلى ToHashSet لتجنب مشكلة الـ WITH في SQL Server
        var ids = request.Items.Select(i => i.Id).Distinct().ToHashSet();
        if (ids.Count != request.Items.Count)
        {
            throw new BusinessRuleException("Duplicate family member Ids in the bulk update request.");
        }

        var members = await _context.FamilyMembers
            .Include(m => m.Beneficiary)
            .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
            .ToListAsync(cancellationToken);

        var memberById = members.ToDictionary(m => m.Id);
        var missingIds = ids.Where(id => !memberById.ContainsKey(id)).ToList();
        if (missingIds.Count > 0)
        {
            throw new NotFoundException($"Family member(s) not found: {string.Join(", ", missingIds)}.");
        }

        foreach (var member in members)
        {
            EnsureCanManageBeneficiary(member.Beneficiary);
        }

        // تم التعديل إلى ToHashSet لتجنب مشكلة الـ WITH في SQL Server
        var headIds = request.Items.Select(i => i.HeadOfHouseholdId).Distinct().ToHashSet();
        var beneficiaries = await _context.Beneficiaries
            .Include(b => b.User)
            .Where(b => headIds.Contains(b.UserId) && !b.IsDeleted)
            .ToListAsync(cancellationToken);

        var beneficiaryByUserId = beneficiaries
            .GroupBy(b => b.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(b => b.IsHeadOfHousehold).ThenByDescending(b => b.Id).First());

        var missingHeads = headIds.Where(id => !beneficiaryByUserId.ContainsKey(id)).ToList();
        if (missingHeads.Count > 0)
        {
            throw new NotFoundException(
                $"Head of household (beneficiary) not found for HeadOfHouseholdId(s): {string.Join(", ", missingHeads)}.");
        }

        foreach (var beneficiary in beneficiaryByUserId.Values)
        {
            EnsureCanManageBeneficiary(beneficiary);
        }

        var results = new List<FamilyMemberResponse>();

        foreach (var item in request.Items)
        {
            var member = memberById[item.Id];
            var target = beneficiaryByUserId[item.HeadOfHouseholdId];

            member.BeneficiaryId = target.Id;
            member.FullName = item.FullName.Trim();
            member.BirthDate = item.DateOfBirth.Date;
            member.Gender = item.Gender;
            member.ClothingSize = item.ClothingSize.Trim();
            member.ShoeSize = item.ShoeSize.Trim();

            results.Add(FamilyMemberMapper.Map(member, target, target.User));
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bulk updated {Count} family members", results.Count);

        return results;
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