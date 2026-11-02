using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.RemoveMember;

public sealed class RemoveHouseholdMemberCommandHandler(IHouseholdRepository households, IHouseholdMemberRepository members)
    : IRequestHandler<RemoveHouseholdMemberCommand, bool>
{
    public async Task<bool> Handle(RemoveHouseholdMemberCommand request, CancellationToken cancellationToken)
    {
        var callerRole = await households.GetMemberRoleAsync(request.HouseholdId, request.RequestingUserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("You are not a member of this household.");
        var target = await members.GetAsync(request.HouseholdId, request.MemberId, cancellationToken)
            ?? throw new InvalidOperationException("Member not found.");
        if (target.Role == MemberRole.Owner) throw new InvalidOperationException("The household owner cannot be removed.");
        if (callerRole != MemberRole.Owner && !(callerRole == MemberRole.Admin && target.Role == MemberRole.Member))
            throw new UnauthorizedAccessException("You do not have permission to remove this member.");
        var removed = await members.RemoveAsync(request.HouseholdId, request.MemberId, request.RequestingUserId, cancellationToken);
        if (!removed) throw new NotFoundException("Member not found.");
        return true;
    }
}
