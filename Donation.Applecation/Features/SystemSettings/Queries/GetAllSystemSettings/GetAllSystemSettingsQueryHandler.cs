using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.SystemSetting.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.SystemSettings.Queries.GetAllSystemSettings;

public sealed class GetAllSystemSettingsQueryHandler
    : IRequestHandler<GetAllSystemSettingsQuery, PaginatedResult<SystemSettingResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllSystemSettingsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<SystemSettingResponse>> Handle(
        GetAllSystemSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.SystemSettings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            var type = request.Type.Trim().ToLower();
            query = query.Where(s => s.Type.ToLower() == type);
        }

        if (request.OrganizationId is Guid orgId && orgId != Guid.Empty)
        {
            query = query.Where(s => s.OrganizationId == orgId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(s =>
                s.Key.ToLower().Contains(term) ||
                s.Value.ToLower().Contains(term) ||
                s.Type.ToLower().Contains(term));
        }

        return await query
            .OrderBy(s => s.Type)
            .ThenBy(s => s.Key)
            .Select(s => new SystemSettingResponse
            {
                Id = s.Id,
                OrganizationId = s.OrganizationId,
                Key = s.Key,
                Value = s.Value,
                Type = s.Type,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
