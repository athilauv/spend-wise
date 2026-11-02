using MediatR;

namespace SpendWise.Application.Features.UserFriends.Commands.RemoveFriend;

public sealed record RemoveFriendCommand(Guid UserId, Guid FriendshipId) : IRequest<bool>;
