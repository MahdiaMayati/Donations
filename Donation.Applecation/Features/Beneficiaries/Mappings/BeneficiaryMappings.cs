using System.Linq.Expressions;
using Donation.Application.DTOs.Beneficiary.Response;
using Donation.Domain.Entities;
using Donation.Domain.Enums;

namespace Donation.Application.Features.Beneficiaries.Mappings;

public static class BeneficiaryMappings
{
    /// <summary>
    /// EF-translatable projection used by all Beneficiary read/write response paths.
    /// </summary>
    public static Expression<Func<Beneficiary, BeneficiaryResponse>> ToResponseExpression() =>
        b => new BeneficiaryResponse
        {
            Id = b.Id,
            UserId = b.UserId,
            VerificationStatus = b.VerificationStatus,
            IsVerified = b.VerificationStatus == VerificationStatus.Verified,
            VerifiedUntil = b.VerifiedUntil,
            CreatedAt = b.CreatedAt,
            IsDeleted = b.IsDeleted,

            FirstName = b.User.FirstName,
            LastName = b.User.LastName,
            Email = b.User.Email ?? string.Empty,
            PhoneNumber = b.User.PhoneNumber,
            DateOfBirth = b.User.DateOfBirth,
            Gender = b.User.Gender,
            PreferredContactMethod = b.User.PreferredContactMethod,
            MaritalStatus = b.User.MaritalStatus,
            EducationalStatus = b.User.EducationalStatus,
            Job = b.User.Job,
            HealthStatus = b.User.HealthStatus,
            OrganizationId = b.User.OrganizationId,

            AddressId = b.AddressId,
            CityName = b.Address.Area.City.Name,
            Street = b.Address.Street,
            AddressDetails = b.Address.Details,

            IdPhotoUrl = b.IdPhotoUrl,
            IsHeadOfHousehold = b.IsHeadOfHousehold
        };
}
