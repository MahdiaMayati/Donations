using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Application.Features.Beneficiaries.Mappings;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Beneficiaries.Commands.RestoreBeneficiary;

public sealed class RestoreBeneficiaryCommandHandler
    : IRequestHandler<RestoreBeneficiaryCommand, BeneficiaryResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<RestoreBeneficiaryCommandHandler> _logger;

    public RestoreBeneficiaryCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<RestoreBeneficiaryCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<BeneficiaryResponse?> Handle(
        RestoreBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var beneficiary = await _context.Beneficiaries
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.IsDeleted, cancellationToken);

        if (beneficiary is null)
        {
            return null;
        }

        EnsureCanManage(beneficiary);

        beneficiary.IsDeleted = false;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Beneficiary restored with Id {BeneficiaryId}", request.Id);

        return await _context.Beneficiaries
            .AsNoTracking()
            .Where(b => b.Id == request.Id)
            .Select(BeneficiaryMappings.ToResponseExpression())
            .FirstAsync(cancellationToken);
    }

    private void EnsureCanManage(Beneficiary beneficiary)
    {
        if (_currentUser.IsAdmin)
        {
            return;
        }

        if (beneficiary.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only manage your own beneficiary profile.");
        }
    }
}
