using MediatR;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.UpdateMemberRole;

public sealed record UpdateHouseholdMemberRoleCommand(Guid RequestingUserId, Guid HouseholdId, Guid MemberId, MemberRole Role) : IRequest<bool>;
