using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMembersBulk;

public sealed record CreateFamilyMemberItem(
    Guid HeadOfHouseholdId,
    string FullName,
    DateTime DateOfBirth,
    bool Gender,
    string ClothingSize,
    string ShoeSize);

public sealed record CreateFamilyMembersBulkCommand(
    IReadOnlyList<CreateFamilyMemberItem> Items) : IRequest<IReadOnlyList<FamilyMemberResponse>>;
