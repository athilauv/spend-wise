using MediatR;

namespace SpendWise.Application.Features.UserFriends.Commands.SendFriendRequest;

public sealed record SendFriendRequestCommand(Guid UserId, Guid FriendUserId) : IRequest<Guid>;
