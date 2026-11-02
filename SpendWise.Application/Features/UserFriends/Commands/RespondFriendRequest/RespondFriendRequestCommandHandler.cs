using MediatR;
using SpendWise.Application.Abstractions.Persistence;

namespace SpendWise.Application.Features.UserFriends.Commands.RespondFriendRequest;

public sealed class RespondFriendRequestCommandHandler(IUserFriendRepository repository) : IRequestHandler<RespondFriendRequestCommand, bool>
{
    public Task<bool> Handle(RespondFriendRequestCommand request, CancellationToken cancellationToken) =>
        repository.RespondAsync(request.FriendshipId, request.UserId, request.Accept, cancellationToken);
}
