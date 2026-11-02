using MediatR;

namespace SpendWise.Application.Features.UserFriends.Commands.RespondFriendRequest;

public sealed record RespondFriendRequestCommand(Guid UserId, Guid FriendshipId, bool Accept) : IRequest<bool>;
