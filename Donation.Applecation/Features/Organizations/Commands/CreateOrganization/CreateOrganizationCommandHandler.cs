using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Organization.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Organizations.Commands.CreateOrganization;

public sealed class CreateOrganizationCommandHandler
    : IRequestHandler<CreateOrganizationCommand, OrganizationResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateOrganizationCommandHandler> _logger;

    public CreateOrganizationCommandHandler(
        IAppDbContext context,
        ILogger<CreateOrganizationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrganizationResponse> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var nameExists = await _context.Organizations
            .AnyAsync(
                o => !o.IsDeleted && o.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("An organization with this name already exists.");
        }

        var organization = new Organization
        {
            Name = name,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Organization created with Id {OrganizationId}", organization.Id);

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
