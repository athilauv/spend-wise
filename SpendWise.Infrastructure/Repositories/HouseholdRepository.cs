using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

public sealed class HouseholdRepository(IDbConnectionFactory factory) : IHouseholdRepository
{
    private const string HCols = "id Id,name Name,owner_user_id OwnerUserId,created_at CreatedAt,created_by CreatedBy,updated_at UpdatedAt,updated_by UpdatedBy,deleted_at DeletedAt,deleted_by DeletedBy";

    public async Task<Household> CreateWithOwnerAsync(Household h, HouseholdMember m, CancellationToken ct)
    {
        const string hs = "INSERT INTO households(id,name,owner_user_id,created_at,created_by,updated_at,updated_by,deleted_at,deleted_by) VALUES(@Id,@Name,@OwnerUserId,@CreatedAt,@CreatedBy,@UpdatedAt,@UpdatedBy,@DeletedAt,@DeletedBy);";
        const string ms = "INSERT INTO household_members(id,household_id,user_id,role,joined_at,is_active,created_at,created_by,updated_at,updated_by,deleted_at,deleted_by) VALUES(@Id,@HouseholdId,@UserId,@Role,@JoinedAt,@IsActive,@CreatedAt,@CreatedBy,@UpdatedAt,@UpdatedBy,@DeletedAt,@DeletedBy);";
        await using var c = factory.CreateConnection();
        await c.OpenAsync(ct);
        await using var tx = await c.BeginTransactionAsync(ct);
        try {
            await c.ExecuteAsync(new CommandDefinition(hs,h,tx,cancellationToken:ct));
            await c.ExecuteAsync(new CommandDefinition(ms,new { m.Id,m.HouseholdId,m.UserId,Role=(int)m.Role,m.JoinedAt,m.IsActive,m.CreatedAt,m.CreatedBy,m.UpdatedAt,m.UpdatedBy,m.DeletedAt,m.DeletedBy },tx,cancellationToken:ct));
            await tx.CommitAsync(ct); return h;
        } catch { await tx.RollbackAsync(ct); throw; }
    }

    public async Task<IReadOnlyList<Household>> GetForUserAsync(Guid userId, CancellationToken ct)
    {
        var sql = $"SELECT h.{HCols.Replace(",", ",h.")} FROM households h JOIN household_members m ON m.household_id=h.id WHERE m.user_id=@userId AND m.is_active=1 AND m.deleted_at IS NULL AND h.deleted_at IS NULL ORDER BY h.created_at DESC;";
        await using var c=factory.CreateConnection();
        var rows=await c.QueryAsync<Household>(new CommandDefinition(sql,new {userId},cancellationToken:ct)); return rows.AsList();
    }

    public async Task<Household?> GetByIdForMemberAsync(Guid householdId, Guid userId, CancellationToken ct)
    {
        var sql=$"SELECT h.{HCols.Replace(",", ",h.")} FROM households h JOIN household_members m ON m.household_id=h.id WHERE h.id=@householdId AND m.user_id=@userId AND m.is_active=1 AND m.deleted_at IS NULL AND h.deleted_at IS NULL;";
        await using var c=factory.CreateConnection();
        return await c.QuerySingleOrDefaultAsync<Household>(new CommandDefinition(sql,new {householdId,userId},cancellationToken:ct));
    }

    public async Task<bool> UpdateAsync(Household h, CancellationToken ct)
    {
        const string sql="UPDATE households SET name=@Name,updated_at=@UpdatedAt,updated_by=@UpdatedBy WHERE id=@Id AND owner_user_id=@UpdatedBy AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,h,cancellationToken:ct))>0;
    }

    public async Task<bool> SoftDeleteAsync(Guid householdId, Guid ownerUserId, CancellationToken ct)
    {
        const string sql="UPDATE households SET deleted_at=@now,deleted_by=@ownerUserId WHERE id=@householdId AND owner_user_id=@ownerUserId AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {householdId,ownerUserId,now=DateTimeOffset.UtcNow},cancellationToken:ct))>0;
    }

    public async Task<MemberRole?> GetMemberRoleAsync(Guid householdId, Guid userId, CancellationToken ct)
    {
        const string sql="SELECT TOP(1) role FROM household_members WHERE household_id=@householdId AND user_id=@userId AND is_active=1 AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); var role=await c.QuerySingleOrDefaultAsync<int?>(new CommandDefinition(sql,new {householdId,userId},cancellationToken:ct));
        return role.HasValue ? (MemberRole)role.Value : null;
    }

    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken ct)
    {
        const string sql="SELECT CAST(IIF(EXISTS(SELECT 1 FROM users WHERE id=@userId AND is_active=1 AND deleted_at IS NULL),1,0) AS bit);";
        await using var c=factory.CreateConnection(); return await c.ExecuteScalarAsync<bool>(new CommandDefinition(sql,new {userId},cancellationToken:ct));
    }
}
