using MediatR;

namespace Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMembersBulk;

public sealed record DeleteFamilyMembersBulkCommand(IReadOnlyList<Guid> Ids) : IRequest<int>;
