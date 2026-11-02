using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

public sealed class CategoryRepository(IDbConnectionFactory factory) : ICategoryRepository
{
    private const string SelectSql = """
        SELECT id Id, user_id UserId, household_id HouseholdId, name Name, icon Icon, color Color,
        is_system IsSystem, is_active IsActive, created_at CreatedAt, created_by CreatedBy,
        updated_at UpdatedAt, updated_by UpdatedBy, deleted_at DeletedAt, deleted_by DeletedBy
        FROM categories
        """;

    public async Task<IReadOnlyList<Category>> GetForUserAsync(Guid userId, Guid householdId, CancellationToken cancellationToken)
    {
        var sql = SelectSql + """
            WHERE deleted_at IS NULL AND is_active=1
              AND (is_system=1 OR user_id=@userId OR user_id IS NULL)
              AND (household_id=@householdId OR household_id IS NULL)
            ORDER BY is_system DESC, name;
            """;
        await using var c = factory.CreateConnection();
        var rows = await c.QueryAsync<Category>(new CommandDefinition(sql, new { userId, householdId }, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<Category?> GetByIdAsync(Guid id, Guid userId, Guid householdId, CancellationToken cancellationToken)
    {
        var sql = SelectSql + """
            WHERE id=@id AND deleted_at IS NULL
              AND (user_id=@userId OR user_id IS NULL)
              AND (household_id=@householdId OR household_id IS NULL);
            """;
        await using var c = factory.CreateConnection();
        return await c.QuerySingleOrDefaultAsync<Category>(new CommandDefinition(sql, new { id, userId, householdId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> NameExistsAsync(string name, Guid userId, Guid householdId, Guid? excludeId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT CAST(IIF(EXISTS(SELECT 1 FROM categories WHERE LOWER(name)=LOWER(@name) AND deleted_at IS NULL
              AND (user_id=@userId OR user_id IS NULL) AND (household_id=@householdId OR household_id IS NULL)
              AND (@excludeId IS NULL OR id<>@excludeId)),1,0) AS bit);
            """;
        await using var c = factory.CreateConnection();
        return await c.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { name, userId, householdId, excludeId }, cancellationToken: cancellationToken));
    }

    public async Task<Category> CreateAsync(Category x, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO categories(id,user_id,household_id,name,icon,color,is_system,is_active,created_at,created_by,updated_at,updated_by,deleted_at,deleted_by)
            VALUES(@Id,@UserId,@HouseholdId,@Name,@Icon,@Color,@IsSystem,@IsActive,@CreatedAt,@CreatedBy,@UpdatedAt,@UpdatedBy,@DeletedAt,@DeletedBy);
            """;
        await using var c = factory.CreateConnection();
        await c.ExecuteAsync(new CommandDefinition(sql, x, cancellationToken: cancellationToken));
        return x;
    }

    public async Task<bool> UpdateAsync(Category x, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE categories SET name=@Name,icon=@Icon,color=@Color,is_active=@IsActive,updated_at=@UpdatedAt,updated_by=@UpdatedBy
            WHERE id=@Id AND deleted_at IS NULL AND is_system=0 AND (user_id=@UserId OR user_id IS NULL)
              AND (household_id=@HouseholdId OR household_id IS NULL);
            """;
        await using var c = factory.CreateConnection();
        return await c.ExecuteAsync(new CommandDefinition(sql, x, cancellationToken: cancellationToken)) > 0;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, Guid userId, Guid householdId, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE categories SET deleted_at=@now,deleted_by=@userId,is_active=0
            WHERE id=@id AND household_id=@householdId AND is_system=0 AND deleted_at IS NULL
              AND (user_id=@userId OR user_id IS NULL);
            """;
        await using var c = factory.CreateConnection();
        return await c.ExecuteAsync(new CommandDefinition(sql, new { id, userId, householdId, now=DateTimeOffset.UtcNow }, cancellationToken: cancellationToken)) > 0;
    }
}
