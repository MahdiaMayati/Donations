using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMember;

public sealed record UpdateFamilyMemberCommand(
    Guid Id,
    Guid HeadOfHouseholdId,
    string FullName,
    DateTime DateOfBirth,
    bool Gender,
    string ClothingSize,
    string ShoeSize) : IRequest<FamilyMemberResponse?>;
