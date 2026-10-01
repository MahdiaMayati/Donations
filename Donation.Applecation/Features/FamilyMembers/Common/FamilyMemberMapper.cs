using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.FamilyMembers.Common;

internal static class FamilyMemberMapper
{
    public static FamilyMemberResponse Map(FamilyMember member, Beneficiary beneficiary, User user) => new()
    {
        Id = member.Id,
        BeneficiaryId = member.BeneficiaryId,
        HeadOfHouseholdId = beneficiary.UserId,
        UserId = beneficiary.UserId,
        AddressId = beneficiary.AddressId,
        ImageUrl = beneficiary.IdPhotoUrl,
        FullName = member.FullName,
        DateOfBirth = member.BirthDate,
        BirthDate = member.BirthDate,
        Gender = member.Gender,
        ClothingSize = member.ClothingSize,
        ShoeSize = member.ShoeSize,
        IsDeleted = member.IsDeleted,
        HeadOfHousehold = MapUser(user)
    };

    public static HeadOfHouseholdUserResponse MapUser(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        PhoneNumber = user.PhoneNumber,
        FirstName = user.FirstName,
        LastName = user.LastName,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        OrganizationId = user.OrganizationId,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender,
        PreferredContactMethod = user.PreferredContactMethod,
        MaritalStatus = user.MaritalStatus,
        EducationalStatus = user.EducationalStatus,
        Job = user.Job,
        HealthStatus = user.HealthStatus
    };
}
