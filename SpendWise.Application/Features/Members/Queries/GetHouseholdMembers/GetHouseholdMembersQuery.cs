using MediatR;
using SpendWise.Application.Features.Members.DTOs;

namespace SpendWise.Application.Features.Members.Queries.GetHouseholdMembers;

public sealed record GetHouseholdMembersQuery(Guid RequestingUserId, Guid HouseholdId) : IRequest<IReadOnlyList<HouseholdMemberDto>>;
