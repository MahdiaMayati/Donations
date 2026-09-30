using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Request;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Application.Features.Beneficiaries.Mappings;
using Donation.Domain.Entities;
using Donation.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;

public sealed class CreateBeneficiaryCommandHandler : IRequestHandler<CreateBeneficiaryCommand, BeneficiaryResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateBeneficiaryCommandHandler> _logger;

    public CreateBeneficiaryCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CreateBeneficiaryCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<BeneficiaryResponse> Handle(CreateBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("Authentication is required.");
        }

        var userId = _currentUser.UserId.Value;

        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        try
        {
            var exists = await _context.Beneficiaries
                .AnyAsync(b => b.UserId == userId && !b.IsDeleted, cancellationToken);

            if (exists)
            {
                throw new ConflictException("A beneficiary profile already exists for this user.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            ApplyUserProfile(user, request.User);

            var city = await ResolveCityAsync(request.City, cancellationToken);
            var area = await ResolveOrCreateAreaAsync(request.Area, city.Id, cancellationToken);
            var address = await ResolveOrCreateAddressAsync(request.Address, area.Id, userId, cancellationToken);

            var beneficiary = new Beneficiary
            {
                UserId = userId,
                AddressId = address.Id,
                IdPhotoUrl = request.IdPhotoUrl.Trim(),
                IsHeadOfHousehold = request.IsHeadOfHousehold,
                VerificationStatus = VerificationStatus.Pending,
                VerifiedUntil = null,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Beneficiary created with Id {BeneficiaryId} (CityId={CityId}, AreaId={AreaId}, AddressId={AddressId})",
                beneficiary.Id, city.Id, area.Id, address.Id);

            return await _context.Beneficiaries
                .AsNoTracking()
                .Where(b => b.Id == beneficiary.Id)
                .Select(BeneficiaryMappings.ToResponseExpression())
                .FirstAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void ApplyUserProfile(User user, CreateBeneficiaryUserRequest profile)
    {
        user.FirstName = profile.FirstName.Trim();
        user.LastName = profile.LastName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(profile.PhoneNumber)
            ? user.PhoneNumber
            : profile.PhoneNumber.Trim();
        user.DateOfBirth = profile.DateOfBirth?.Date;
        user.Gender = profile.Gender;
        user.PreferredContactMethod = profile.PreferredContactMethod.Trim();
        user.MaritalStatus = profile.MaritalStatus.Trim();
        user.EducationalStatus = profile.EducationalStatus.Trim();
        user.Job = profile.Job.Trim();
        user.HealthStatus = profile.HealthStatus.Trim();
    }

    private async Task<City> ResolveCityAsync(
        CreateBeneficiaryCityRequest cityRequest,
        CancellationToken cancellationToken)
    {
        return await _context.Cities
                   .FirstOrDefaultAsync(c => c.Id == cityRequest.Id, cancellationToken)
               ?? throw new NotFoundException("City not found.");
    }

    private async Task<Area> ResolveOrCreateAreaAsync(
        CreateBeneficiaryAreaRequest areaRequest,
        Guid cityId,
        CancellationToken cancellationToken)
    {
        var name = areaRequest.Name.Trim();
        var existing = await _context.Areas
            .FirstOrDefaultAsync(
                a => a.CityId == cityId && a.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var areaToCreate = new Area
        {
            CityId = cityId,
            Name = name
        };

        _context.Areas.Add(areaToCreate);
        await _context.SaveChangesAsync(cancellationToken);
        return areaToCreate;
    }

    private async Task<Address> ResolveOrCreateAddressAsync(
        CreateBeneficiaryAddressRequest addressRequest,
        Guid areaId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var street = addressRequest.Street.Trim();
        var details = addressRequest.Details.Trim();
        var latitude = addressRequest.Latitude;
        var longitude = addressRequest.Longitude;

        var existing = await _context.Addresses
            .FirstOrDefaultAsync(
                a => a.UserId == userId
                     && a.AreaId == areaId
                     && a.Street.ToLower() == street.ToLower()
                     && a.Details.ToLower() == details.ToLower(),
                cancellationToken);

        if (existing is not null)
        {
            // Refresh coordinates if the caller sent an updated location for the same street/details.
            existing.Latitude = latitude;
            existing.Longitude = longitude;
            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        var addressToCreate = new Address
        {
            AreaId = areaId,
            UserId = userId,
            Street = street,
            Details = details,
            Latitude = latitude,
            Longitude = longitude
        };

        _context.Addresses.Add(addressToCreate);
        await _context.SaveChangesAsync(cancellationToken);
        return addressToCreate;
    }
}
