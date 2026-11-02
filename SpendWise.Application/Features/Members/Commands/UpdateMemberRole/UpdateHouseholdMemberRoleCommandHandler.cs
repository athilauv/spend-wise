using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.UpdateMemberRole;

public sealed class UpdateHouseholdMemberRoleCommandHandler(IHouseholdRepository households, IHouseholdMemberRepository members)
    : IRequestHandler<UpdateHouseholdMemberRoleCommand, bool>
{
    public async Task<bool> Handle(UpdateHouseholdMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var callerRole = await households.GetMemberRoleAsync(request.HouseholdId, request.RequestingUserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("You are not a member of this household.");
        if (callerRole != MemberRole.Owner) throw new UnauthorizedAccessException("Only the owner can change member roles.");
        var target = await members.GetAsync(request.HouseholdId, request.MemberId, cancellationToken)
            ?? throw new InvalidOperationException("Member not found.");
        if (target.Role == MemberRole.Owner) throw new InvalidOperationException("The household owner role cannot be changed here.");
        if (target.UserId == request.RequestingUserId) throw new InvalidOperationException("You cannot change your own role.");
        return await members.UpdateRoleAsync(request.HouseholdId, request.MemberId, request.Role, request.RequestingUserId, cancellationToken);
    }
}
