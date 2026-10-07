using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.SystemSetting.Response;
using Donation.Application.Features.SystemSettings.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.SystemSettings.Commands.CreateSystemSetting;

public sealed class CreateSystemSettingCommandHandler
    : IRequestHandler<CreateSystemSettingCommand, SystemSettingResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateSystemSettingCommandHandler> _logger;

    public CreateSystemSettingCommandHandler(
        IAppDbContext context,
        ILogger<CreateSystemSettingCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SystemSettingResponse> Handle(
        CreateSystemSettingCommand request,
        CancellationToken cancellationToken)
    {
        var organizationId = NormalizeOrganizationId(request.OrganizationId);
        if (organizationId.HasValue)
        {
            await EnsureOrganizationExistsAsync(organizationId.Value, cancellationToken);
        }

        var key = request.Key.Trim();
        var type = request.Type.Trim();
        var value = request.Value.Trim();

        var keyExists = await _context.SystemSettings.AnyAsync(
            s => s.OrganizationId == organizationId && s.Key.ToLower() == key.ToLower(),
            cancellationToken);

        if (keyExists)
        {
            throw new ConflictException(
                organizationId.HasValue
                    ? $"A system setting with key '{key}' already exists for this organization."
                    : $"A global system setting with key '{key}' already exists.");
        }

        var entity = new SystemSetting
        {
            OrganizationId = organizationId,
            Key = key,
            Value = value,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        _context.SystemSettings.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("SystemSetting created with Id {SystemSettingId}", entity.Id);
        return SystemSettingMapper.Map(entity);
    }

    private async Task EnsureOrganizationExistsAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var exists = await _context.Organizations
            .AnyAsync(o => o.Id == organizationId && !o.IsDeleted, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Organization not found.");
        }
    }

    private static Guid? NormalizeOrganizationId(Guid? organizationId)
        => organizationId is Guid id && id != Guid.Empty ? id : null;
}
