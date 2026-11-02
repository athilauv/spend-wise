using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Members.DTOs;

namespace SpendWise.Application.Features.Members.Queries.GetHouseholdMembers;

public sealed class GetHouseholdMembersQueryHandler(IHouseholdRepository households, IHouseholdMemberRepository members)
    : IRequestHandler<GetHouseholdMembersQuery, IReadOnlyList<HouseholdMemberDto>>
{
    public async Task<IReadOnlyList<HouseholdMemberDto>> Handle(GetHouseholdMembersQuery request, CancellationToken cancellationToken)
    {
        if (await households.GetMemberRoleAsync(request.HouseholdId, request.RequestingUserId, cancellationToken) is null)
            throw new NotFoundException("Household not found or user is not a member.");
        var items = await members.GetByHouseholdAsync(request.HouseholdId, cancellationToken);
        return items;
    }
}
