using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;

public sealed class DeleteFamilyMemberCommandHandler : IRequestHandler<DeleteFamilyMemberCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteFamilyMemberCommandHandler> _logger;

    public DeleteFamilyMemberCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<DeleteFamilyMemberCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteFamilyMemberCommand request, CancellationToken cancellationToken)
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
            return false;
        }

        EnsureCanManageBeneficiary(member.Beneficiary);

        member.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FamilyMember soft-deleted with Id {FamilyMemberId}", request.Id);

        return true;
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
