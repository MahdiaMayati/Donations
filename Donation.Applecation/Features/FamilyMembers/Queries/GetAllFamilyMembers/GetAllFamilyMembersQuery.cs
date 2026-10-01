using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.FamilyMember.Response;
using MediatR;

namespace Donation.Application.Features.FamilyMembers.Queries.GetAllFamilyMembers;

public sealed record GetAllFamilyMembersQuery(
    Guid? BeneficiaryId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<FamilyMemberResponse>>;
