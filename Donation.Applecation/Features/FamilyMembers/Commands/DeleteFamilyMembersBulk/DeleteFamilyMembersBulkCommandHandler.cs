using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMembersBulk;

public sealed class DeleteFamilyMembersBulkCommandHandler : IRequestHandler<DeleteFamilyMembersBulkCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteFamilyMembersBulkCommandHandler> _logger;

    public DeleteFamilyMembersBulkCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<DeleteFamilyMembersBulkCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteFamilyMembersBulkCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        // تم التعديل إلى ToHashSet لمنع مشكلة الـ WITH في SQL Server
        var uniqueIds = request.Ids.Distinct().ToHashSet();

        var members = await _context.FamilyMembers
            .Include(m => m.Beneficiary)
            .Where(m => uniqueIds.Contains(m.Id) && !m.IsDeleted)
            .ToListAsync(cancellationToken);

        var missing = uniqueIds.Where(id => members.All(m => m.Id != id)).ToList();
        if (missing.Count > 0)
        {
            throw new NotFoundException($"Family member(s) not found: {string.Join(", ", missing)}.");
        }

        foreach (var member in members)
        {
            EnsureCanManageBeneficiary(member.Beneficiary);
            member.IsDeleted = true;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bulk soft-deleted {Count} family members", members.Count);

        return members.Count;
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