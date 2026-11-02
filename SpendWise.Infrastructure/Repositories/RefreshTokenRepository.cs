using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Persistence.Connections;
namespace SpendWise.Infrastructure.Repositories;
public sealed class RefreshTokenRepository(IDbConnectionFactory factory) : IRefreshTokenRepository
{
    public async Task SaveAsync(RefreshToken x, CancellationToken ct)
    {
        const string sql = """
        INSERT INTO refresh_tokens(id,user_id,token_hash,expires_at,revoked_at,replaced_by_token_id,created_at,created_by)
        VALUES(@Id,@UserId,@TokenHash,@ExpiresAt,@RevokedAt,@ReplacedByTokenId,@CreatedAt,@CreatedBy);
        """;
        await using var c = factory.CreateConnection();
        await c.ExecuteAsync(new CommandDefinition(sql, x, cancellationToken: ct));
    }
    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct)
    {
        const string sql = """
        SELECT id Id,user_id UserId,token_hash TokenHash,expires_at ExpiresAt,revoked_at RevokedAt,
               replaced_by_token_id ReplacedByTokenId,created_at CreatedAt,created_by CreatedBy
        FROM refresh_tokens WHERE token_hash=@tokenHash;
        """;
        await using var c = factory.CreateConnection();
        return await c.QuerySingleOrDefaultAsync<RefreshToken>(new CommandDefinition(sql, new { tokenHash }, cancellationToken: ct));
    }
    public async Task RevokeAsync(Guid id, DateTimeOffset revokedAt, Guid? replacedByTokenId, CancellationToken ct)
    {
        const string sql = "UPDATE refresh_tokens SET revoked_at=@revokedAt,replaced_by_token_id=@replacedByTokenId WHERE id=@id AND revoked_at IS NULL;";
        await using var c = factory.CreateConnection();
        await c.ExecuteAsync(new CommandDefinition(sql, new { id, revokedAt, replacedByTokenId }, cancellationToken: ct));
    }
}
