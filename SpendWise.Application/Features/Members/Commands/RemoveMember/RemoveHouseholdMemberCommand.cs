using MediatR;

namespace SpendWise.Application.Features.Members.Commands.RemoveMember;

public sealed record RemoveHouseholdMemberCommand(Guid RequestingUserId, Guid HouseholdId, Guid MemberId) : IRequest<bool>;
