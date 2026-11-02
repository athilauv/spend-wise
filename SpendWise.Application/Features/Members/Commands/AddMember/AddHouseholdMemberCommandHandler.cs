using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.AddMember;

public sealed class AddHouseholdMemberCommandHandler(IHouseholdRepository households, IHouseholdMemberRepository members)
    : IRequestHandler<AddHouseholdMemberCommand, Guid>
{
    public async Task<Guid> Handle(AddHouseholdMemberCommand request, CancellationToken cancellationToken)
    {
        var role = await households.GetMemberRoleAsync(request.HouseholdId, request.RequestingUserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("You are not a member of this household.");
        if (role is not (MemberRole.Owner or MemberRole.Admin)) throw new UnauthorizedAccessException("Only owners or admins can add members.");
        if (request.UserId == request.RequestingUserId && await members.IsMemberAsync(request.HouseholdId, request.UserId, cancellationToken))
            throw new InvalidOperationException("This user is already a member.");
        if (!await members.UserExistsAsync(request.UserId, cancellationToken)) throw new InvalidOperationException("The target user does not exist.");
        if (await members.IsMemberAsync(request.HouseholdId, request.UserId, cancellationToken)) throw new InvalidOperationException("The user is already a member.");
        if (request.Role == MemberRole.Admin && role != MemberRole.Owner) throw new UnauthorizedAccessException("Only the owner can assign the admin role.");
        var now = DateTimeOffset.UtcNow;
        var member = new HouseholdMember { Id = Guid.NewGuid(), HouseholdId = request.HouseholdId, UserId = request.UserId, Role = request.Role, JoinedAt = now, IsActive = true, CreatedAt = now, CreatedBy = request.RequestingUserId };
        if (!await members.AddAsync(member, cancellationToken)) throw new InvalidOperationException("Unable to add household member.");
        return member.Id;
    }
}
