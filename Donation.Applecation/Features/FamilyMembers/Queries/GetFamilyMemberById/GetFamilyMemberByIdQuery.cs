using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Queries.GetFamilyMemberById;

public sealed record GetFamilyMemberByIdQuery(Guid Id) : IRequest<FamilyMemberResponse?>;
