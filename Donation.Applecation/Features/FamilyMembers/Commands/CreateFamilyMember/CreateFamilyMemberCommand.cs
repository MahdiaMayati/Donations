using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMember;

public sealed record CreateFamilyMemberCommand(
    Guid HeadOfHouseholdId,
    string FullName,
    DateTime DateOfBirth,
    bool Gender,
    string ClothingSize,
    string ShoeSize) : IRequest<FamilyMemberResponse>;
