using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMembersBulk;

public sealed record UpdateFamilyMemberItem(
    Guid Id,
    Guid HeadOfHouseholdId,
    string FullName,
    DateTime DateOfBirth,
    bool Gender,
    string ClothingSize,
    string ShoeSize);

public sealed record UpdateFamilyMembersBulkCommand(
    IReadOnlyList<UpdateFamilyMemberItem> Items) : IRequest<IReadOnlyList<FamilyMemberResponse>>;
