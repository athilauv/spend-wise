using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using UserFriendResponseDto = SpendWise.Application.Features.UserFriends.DTOs.UserFriendDto;

namespace SpendWise.Application.Features.UserFriends.Queries.GetFriends;

public sealed class GetFriendsQueryHandler(IUserFriendRepository repository)
    : IRequestHandler<GetFriendsQuery, IReadOnlyList<UserFriendResponseDto>>
{
    public async Task<IReadOnlyList<UserFriendResponseDto>> Handle(
        GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetFriendsAsync(request.UserId, cancellationToken);
        return items.Select(x => new UserFriendResponseDto(
            x.Id, x.UserId, x.FriendUserId, x.FriendName, x.FriendEmail, x.Status, x.CreatedAt)).ToArray();
    }
}
