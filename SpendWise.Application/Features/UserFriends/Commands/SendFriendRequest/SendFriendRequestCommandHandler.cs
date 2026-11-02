using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.UserFriends.Commands.SendFriendRequest;

public sealed class SendFriendRequestCommandHandler(IUserFriendRepository repository) : IRequestHandler<SendFriendRequestCommand, Guid>
{
    public async Task<Guid> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        // Check whether the friend user exists.
        if (!await repository.UserExistsAsync(request.FriendUserId, cancellationToken)) throw new NotFoundException("Friend user not found.");
        // Check whether a friendship or request already exists.
        if (await repository.RelationshipExistsAsync(request.UserId, request.FriendUserId, cancellationToken)) throw new ConflictException("A friendship or request already exists.");
        // Create a new friend request.
        return await repository.CreateRequestAsync(request.UserId, request.FriendUserId, cancellationToken);
    }
}
