using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

public sealed class UserFriendRepository(IDbConnectionFactory factory) : IUserFriendRepository
{
    public async Task<IReadOnlyList<UserFriendDto>> GetFriendsAsync(Guid userId,CancellationToken ct)
    {
        const string sql="""
            SELECT f.id Id,f.user_id UserId,f.friend_user_id FriendUserId,u.full_name FriendName,u.email FriendEmail,f.status Status,f.created_at CreatedAt
            FROM user_friends f JOIN users u ON u.id=f.friend_user_id
            WHERE f.user_id=@userId AND f.status='Accepted' AND f.deleted_at IS NULL
            UNION ALL
            SELECT f.id Id,f.friend_user_id UserId,f.user_id FriendUserId,u.full_name FriendName,u.email FriendEmail,f.status Status,f.created_at CreatedAt
            FROM user_friends f JOIN users u ON u.id=f.user_id
            WHERE f.friend_user_id=@userId AND f.status='Accepted' AND f.deleted_at IS NULL;
            """;
        await using var c=factory.CreateConnection(); var rows=await c.QueryAsync<UserFriendDto>(new CommandDefinition(sql,new {userId},cancellationToken:ct)); return rows.AsList();
    }
    public Task<bool> UserExistsAsync(Guid userId,CancellationToken ct) => ExistsAsync("SELECT CAST(IIF(EXISTS(SELECT 1 FROM users WHERE id=@userId AND is_active=1 AND deleted_at IS NULL),1,0) AS bit);",new {userId},ct);
    public Task<bool> RelationshipExistsAsync(Guid userId,Guid friendUserId,CancellationToken ct) => ExistsAsync("SELECT CAST(IIF(EXISTS(SELECT 1 FROM user_friends WHERE deleted_at IS NULL AND status IN ('Pending','Accepted') AND ((user_id=@userId AND friend_user_id=@friendUserId) OR (user_id=@friendUserId AND friend_user_id=@userId))),1,0) AS bit);",new {userId,friendUserId},ct);
    private async Task<bool> ExistsAsync(string sql,object args,CancellationToken ct) { await using var c=factory.CreateConnection(); return await c.ExecuteScalarAsync<bool>(new CommandDefinition(sql,args,cancellationToken:ct)); }
    public async Task<Guid> CreateRequestAsync(Guid userId,Guid friendUserId,CancellationToken ct)
    {
        var id=Guid.NewGuid(); const string sql="INSERT INTO user_friends(id,user_id,friend_user_id,status,created_at,created_by) VALUES(@id,@userId,@friendUserId,@Status,@now,@userId);";
        await using var c=factory.CreateConnection(); await c.ExecuteAsync(new CommandDefinition(sql,new {id,userId,friendUserId,Status=UserFriend.Pending,now=DateTimeOffset.UtcNow},cancellationToken:ct)); return id;
    }
    public async Task<bool> RespondAsync(Guid friendshipId,Guid userId,bool accept,CancellationToken ct)
    {
        const string sql="UPDATE user_friends SET status=@status,responded_at=@respondedAt,updated_at=@now,updated_by=@userId WHERE id=@friendshipId AND friend_user_id=@userId AND status='Pending' AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {friendshipId,userId,status=accept?UserFriend.Accepted:UserFriend.Declined,respondedAt=DateTimeOffset.UtcNow,now=DateTimeOffset.UtcNow},cancellationToken:ct))>0;
    }
    public async Task<bool> RemoveAsync(Guid friendshipId,Guid userId,CancellationToken ct)
    {
        const string sql="UPDATE user_friends SET deleted_at=@now,deleted_by=@userId WHERE id=@friendshipId AND (user_id=@userId OR friend_user_id=@userId) AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {friendshipId,userId,now=DateTimeOffset.UtcNow},cancellationToken:ct))>0;
    }
}
