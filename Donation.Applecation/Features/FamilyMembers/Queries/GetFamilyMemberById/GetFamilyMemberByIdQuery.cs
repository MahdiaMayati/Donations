using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Queries.GetFamilyMemberById;

public sealed record GetFamilyMemberByIdQuery(int Id) : IRequest<FamilyMemberResponse?>;
