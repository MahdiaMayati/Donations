using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Application.Features.Beneficiaries.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Beneficiaries.Queries.GetBeneficiaryById;

public sealed class GetBeneficiaryByIdQueryHandler : IRequestHandler<GetBeneficiaryByIdQuery, BeneficiaryResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBeneficiaryByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BeneficiaryResponse?> Handle(GetBeneficiaryByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var beneficiary = await _context.Beneficiaries
            .AsNoTracking()
            .Where(b => b.Id == request.Id && !b.IsDeleted)
            .Select(BeneficiaryMappings.ToResponseExpression())
            .FirstOrDefaultAsync(cancellationToken);

        if (beneficiary is null)
        {
            return null;
        }

        if (!_currentUser.IsAdmin && beneficiary.UserId != _currentUser.UserId)
        {
            throw new ForbiddenException("You can only view your own beneficiary profile.");
        }

        return beneficiary;
    }
}
