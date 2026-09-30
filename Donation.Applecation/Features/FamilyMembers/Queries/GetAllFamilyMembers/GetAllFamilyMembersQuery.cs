using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Queries.GetAllFamilyMembers;

public sealed record GetAllFamilyMembersQuery(Guid? BeneficiaryId = null) : IRequest<IReadOnlyList<FamilyMemberResponse>>;
