using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.SystemSetting.Response;
using Donation.Application.Features.SystemSettings.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.SystemSettings.Commands.UpdateSystemSetting;

public sealed class UpdateSystemSettingCommandHandler
    : IRequestHandler<UpdateSystemSettingCommand, SystemSettingResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateSystemSettingCommandHandler> _logger;

    public UpdateSystemSettingCommandHandler(
        IAppDbContext context,
        ILogger<UpdateSystemSettingCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SystemSettingResponse?> Handle(
        UpdateSystemSettingCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var organizationId = NormalizeOrganizationId(request.OrganizationId);
        if (organizationId.HasValue)
        {
            var orgExists = await _context.Organizations
                .AnyAsync(o => o.Id == organizationId.Value && !o.IsDeleted, cancellationToken);
            if (!orgExists)
            {
                throw new NotFoundException("Organization not found.");
            }
        }

        var key = request.Key.Trim();
        var keyExists = await _context.SystemSettings.AnyAsync(
            s => s.Id != request.Id
                 && s.OrganizationId == organizationId
                 && s.Key.ToLower() == key.ToLower(),
            cancellationToken);

        if (keyExists)
        {
            throw new ConflictException(
                organizationId.HasValue
                    ? $"A system setting with key '{key}' already exists for this organization."
                    : $"A global system setting with key '{key}' already exists.");
        }

        entity.OrganizationId = organizationId;
        entity.Key = key;
        entity.Value = request.Value.Trim();
        entity.Type = request.Type.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("SystemSetting {SystemSettingId} updated", entity.Id);
        return SystemSettingMapper.Map(entity);
    }

    private static Guid? NormalizeOrganizationId(Guid? organizationId)
        => organizationId is Guid id && id != Guid.Empty ? id : null;
}
