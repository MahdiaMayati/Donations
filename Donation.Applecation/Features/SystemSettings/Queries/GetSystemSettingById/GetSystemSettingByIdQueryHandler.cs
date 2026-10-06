using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.SystemSetting.Response;
using Donation.Application.Features.SystemSettings.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.SystemSettings.Queries.GetSystemSettingById;

public sealed class GetSystemSettingByIdQueryHandler
    : IRequestHandler<GetSystemSettingByIdQuery, SystemSettingResponse?>
{
    private readonly IAppDbContext _context;

    public GetSystemSettingByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSettingResponse?> Handle(
        GetSystemSettingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        return entity is null ? null : SystemSettingMapper.Map(entity);
    }
}
