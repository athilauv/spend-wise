namespace SpendWise.Application.Abstractions.Persistence;

public sealed record UserFriendDto(Guid Id, Guid UserId, Guid FriendUserId, string FriendName, string FriendEmail, string Status, DateTimeOffset CreatedAt);

public interface IUserFriendRepository
{
    Task<IReadOnlyList<UserFriendDto>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> RelationshipExistsAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken);
    Task<Guid> CreateRequestAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken);
    Task<bool> RespondAsync(Guid friendshipId, Guid userId, bool accept, CancellationToken cancellationToken);
    Task<bool> RemoveAsync(Guid friendshipId, Guid userId, CancellationToken cancellationToken);
}
