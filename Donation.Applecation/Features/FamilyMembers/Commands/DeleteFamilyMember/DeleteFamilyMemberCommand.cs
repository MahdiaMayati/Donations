using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;

public sealed record DeleteFamilyMemberCommand(Guid Id) : IRequest<bool>;
