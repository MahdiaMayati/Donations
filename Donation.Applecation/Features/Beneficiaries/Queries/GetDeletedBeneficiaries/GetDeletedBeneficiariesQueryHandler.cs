using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Application.Features.Beneficiaries.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Beneficiaries.Queries.GetDeletedBeneficiaries;

public sealed class GetDeletedBeneficiariesQueryHandler
    : IRequestHandler<GetDeletedBeneficiariesQuery, IReadOnlyList<BeneficiaryResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeletedBeneficiariesQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<BeneficiaryResponse>> Handle(
        GetDeletedBeneficiariesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        // Global query filter hides soft-deleted rows; bypass it for this admin/recovery view.
        var query = _context.Beneficiaries
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(b => b.IsDeleted);

        if (!_currentUser.IsAdmin)
        {
            query = query.Where(b => b.UserId == _currentUser.UserId.Value);
        }

        return await query
            .OrderByDescending(b => b.Id)
            .Select(BeneficiaryMappings.ToResponseExpression())
            .ToListAsync(cancellationToken);
    }
}
