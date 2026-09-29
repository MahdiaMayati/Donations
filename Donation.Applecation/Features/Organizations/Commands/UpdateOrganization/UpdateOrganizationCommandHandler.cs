using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Organization.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Organizations.Commands.UpdateOrganization;

public sealed class UpdateOrganizationCommandHandler
    : IRequestHandler<UpdateOrganizationCommand, OrganizationResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateOrganizationCommandHandler> _logger;

    public UpdateOrganizationCommandHandler(
        IAppDbContext context,
        ILogger<UpdateOrganizationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrganizationResponse?> Handle(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(
                o => o.Id == request.Id && !o.IsDeleted,
                cancellationToken);

        if (organization is null)
        {
            return null;
        }

        var name = request.Name.Trim();

        var nameExists = await _context.Organizations
            .AnyAsync(
                o => o.Id != request.Id
                     && !o.IsDeleted
                     && o.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("An organization with this name already exists.");
        }

        organization.Name = name;
        organization.IsActive = request.IsActive;
        organization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Organization updated with Id {OrganizationId}", organization.Id);

        return Map(organization);
    }

    private static OrganizationResponse Map(Organization organization) => new()
    {
        Id = organization.Id,
        Name = organization.Name,
        IsActive = organization.IsActive,
        CreatedAt = organization.CreatedAt,
        UpdatedAt = organization.UpdatedAt
    };
}
