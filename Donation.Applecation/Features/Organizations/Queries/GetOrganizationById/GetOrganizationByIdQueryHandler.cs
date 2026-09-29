using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Organization.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Organizations.Queries.GetOrganizationById;

public sealed class GetOrganizationByIdQueryHandler
    : IRequestHandler<GetOrganizationByIdQuery, OrganizationResponse?>
{
    private readonly IAppDbContext _context;

    public GetOrganizationByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationResponse?> Handle(
        GetOrganizationByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Id == request.Id && !o.IsDeleted)
            .Select(o => new OrganizationResponse
            {
                Id = o.Id,
                Name = o.Name,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
