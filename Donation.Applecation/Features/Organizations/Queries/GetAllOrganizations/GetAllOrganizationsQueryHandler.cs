using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Organization.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQueryHandler
    : IRequestHandler<GetAllOrganizationsQuery, PaginatedResult<OrganizationResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllOrganizationsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<OrganizationResponse>> Handle(
        GetAllOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Organizations
            .AsNoTracking()
            .Where(o => !o.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(o => o.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationResponse
            {
                Id = o.Id,
                Name = o.Name,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
