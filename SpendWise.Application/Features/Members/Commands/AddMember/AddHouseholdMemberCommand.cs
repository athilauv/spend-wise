using MediatR;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.AddMember;

public sealed record AddHouseholdMemberCommand(Guid RequestingUserId, Guid HouseholdId, Guid UserId, MemberRole Role) : IRequest<Guid>;
