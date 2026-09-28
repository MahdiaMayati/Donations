using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;

public sealed record DeleteFamilyMemberCommand(int Id) : IRequest<bool>;
