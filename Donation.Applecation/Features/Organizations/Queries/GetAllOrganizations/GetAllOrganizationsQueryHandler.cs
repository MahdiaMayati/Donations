using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Common;
using Donation.Application.DTOs.Organization.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQueryHandler
    : IRequestHandler<GetAllOrganizationsQuery, PagedResult<OrganizationResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllOrganizationsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrganizationResponse>> Handle(
        GetAllOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Organizations
            .AsNoTracking()
            .Where(o => !o.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(o => o.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrganizationResponse
            {
                Id = o.Id,
                Name = o.Name,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<OrganizationResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
