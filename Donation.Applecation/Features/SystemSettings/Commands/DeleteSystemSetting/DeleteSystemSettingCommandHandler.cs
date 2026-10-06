using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.SystemSettings.Commands.DeleteSystemSetting;

public sealed class DeleteSystemSettingCommandHandler
    : IRequestHandler<DeleteSystemSettingCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteSystemSettingCommandHandler> _logger;

    public DeleteSystemSettingCommandHandler(
        IAppDbContext context,
        ILogger<DeleteSystemSettingCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteSystemSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _context.SystemSettings.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("SystemSetting {SystemSettingId} deleted", entity.Id);
        return true;
    }
}
