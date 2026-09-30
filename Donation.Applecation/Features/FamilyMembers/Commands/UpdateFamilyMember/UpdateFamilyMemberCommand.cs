using Donation.Application.DTOs.FamilyMember.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMember;

public sealed record UpdateFamilyMemberCommand(
    Guid Id,
    Guid BeneficiaryId,
    string FullName,
    DateTime BirthDate,
    Gender Gender,
    ClothingSize ClothingSize,
    string ShoeSize) : IRequest<FamilyMemberResponse?>;
