using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.UserFriends.Commands.RemoveFriend;

public sealed class RemoveFriendCommandHandler(IUserFriendRepository repository) : IRequestHandler<RemoveFriendCommand, bool>
{
    public async Task<bool> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        if (!await repository.RemoveAsync(request.FriendshipId, request.UserId, cancellationToken))
            throw new NotFoundException("Friendship or request not found.");
        return true;
    }
}
