using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Members.DTOs;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

public sealed class HouseholdMemberRepository(IDbConnectionFactory factory) : IHouseholdMemberRepository
{
    private const string Cols="id Id,household_id HouseholdId,user_id UserId,role Role,joined_at JoinedAt,is_active IsActive,created_at CreatedAt,created_by CreatedBy,updated_at UpdatedAt,updated_by UpdatedBy,deleted_at DeletedAt,deleted_by DeletedBy";

    public async Task<IReadOnlyList<HouseholdMemberDto>> GetByHouseholdAsync(Guid householdId, CancellationToken ct)
    {
        const string sql = """
            SELECT m.id Id, m.household_id HouseholdId, m.user_id UserId, u.full_name FullName, u.email Email, m.role Role, m.joined_at JoinedAt, m.is_active IsActive
            FROM household_members m
            JOIN users u ON u.id = m.user_id
            WHERE m.household_id=@householdId AND m.is_active=1 AND m.deleted_at IS NULL AND u.deleted_at IS NULL
            ORDER BY m.joined_at;
            """;
        await using var c = factory.CreateConnection();
        var rows = await c.QueryAsync<HouseholdMemberDto>(new CommandDefinition(sql, new { householdId }, cancellationToken: ct));
        return rows.AsList();
    }
    public async Task<HouseholdMember?> GetAsync(Guid householdId,Guid memberId,CancellationToken ct)
    {
        var sql=$"SELECT {Cols} FROM household_members WHERE household_id=@householdId AND id=@memberId AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.QuerySingleOrDefaultAsync<HouseholdMember>(new CommandDefinition(sql,new {householdId,memberId},cancellationToken:ct));
    }
    public Task<bool> IsMemberAsync(Guid householdId,Guid userId,CancellationToken ct) => ExistsAsync("SELECT CAST(IIF(EXISTS(SELECT 1 FROM household_members WHERE household_id=@householdId AND user_id=@userId AND is_active=1 AND deleted_at IS NULL),1,0) AS bit);",new {householdId,userId},ct);
    public Task<bool> UserExistsAsync(Guid userId,CancellationToken ct) => ExistsAsync("SELECT CAST(IIF(EXISTS(SELECT 1 FROM users WHERE id=@userId AND is_active=1 AND deleted_at IS NULL),1,0) AS bit);",new {userId},ct);
    private async Task<bool> ExistsAsync(string sql,object args,CancellationToken ct) { await using var c=factory.CreateConnection(); return await c.ExecuteScalarAsync<bool>(new CommandDefinition(sql,args,cancellationToken:ct)); }

    public async Task<bool> AddAsync(HouseholdMember m,CancellationToken ct)
    {
        const string sql="INSERT INTO household_members(id,household_id,user_id,role,joined_at,is_active,created_at,created_by,updated_at,updated_by,deleted_at,deleted_by) VALUES(@Id,@HouseholdId,@UserId,@Role,@JoinedAt,@IsActive,@CreatedAt,@CreatedBy,@UpdatedAt,@UpdatedBy,@DeletedAt,@DeletedBy);";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {m.Id,m.HouseholdId,m.UserId,Role=(int)m.Role,m.JoinedAt,m.IsActive,m.CreatedAt,m.CreatedBy,m.UpdatedAt,m.UpdatedBy,m.DeletedAt,m.DeletedBy},cancellationToken:ct))>0;
    }
    public async Task<bool> UpdateRoleAsync(Guid householdId,Guid memberId,MemberRole role,Guid updatedBy,CancellationToken ct)
    {
        const string sql="UPDATE household_members SET role=@role,updated_at=@now,updated_by=@updatedBy WHERE household_id=@householdId AND id=@memberId AND deleted_at IS NULL AND is_active=1 AND role<>@owner;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {householdId,memberId,role=(int)role,updatedBy,now=DateTimeOffset.UtcNow,owner=(int)MemberRole.Owner},cancellationToken:ct))>0;
    }
    public async Task<bool> RemoveAsync(Guid householdId,Guid memberId,Guid deletedBy,CancellationToken ct)
    {
        const string sql="UPDATE household_members SET deleted_at=@now,deleted_by=@deletedBy,is_active=0 WHERE household_id=@householdId AND id=@memberId AND deleted_at IS NULL AND role<>@owner;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {householdId,memberId,deletedBy,now=DateTimeOffset.UtcNow,owner=(int)MemberRole.Owner},cancellationToken:ct))>0;
    }
    public async Task<MemberRole?> GetRoleAsync(Guid householdId,Guid userId,CancellationToken ct)
    {
        const string sql="SELECT TOP(1) role FROM household_members WHERE household_id=@householdId AND user_id=@userId AND is_active=1 AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); var r=await c.QuerySingleOrDefaultAsync<int?>(new CommandDefinition(sql,new {householdId,userId},cancellationToken:ct)); return r.HasValue?(MemberRole)r.Value:null;
    }
}
