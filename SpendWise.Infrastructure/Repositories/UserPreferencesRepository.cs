using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

public sealed class UserPreferencesRepository(IDbConnectionFactory factory) : IUserPreferenceRepository
{
    public async Task<UserPreference?> GetByUserIdAsync(Guid userId,CancellationToken ct)
    {
        const string sql="SELECT id Id,user_id UserId,currency_code CurrencyCode,time_zone TimeZone,date_range_preference DateRangePreference,created_at CreatedAt,created_by CreatedBy,updated_at UpdatedAt,updated_by UpdatedBy,deleted_at DeletedAt,deleted_by DeletedBy FROM user_preferences WHERE user_id=@userId AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.QuerySingleOrDefaultAsync<UserPreference>(new CommandDefinition(sql,new {userId},cancellationToken:ct));
    }
    public async Task<UserPreference> UpsertAsync(UserPreference x,CancellationToken ct)
    {
        const string sql="""
            IF EXISTS(SELECT 1 FROM user_preferences WHERE user_id=@UserId AND deleted_at IS NULL)
              UPDATE user_preferences SET currency_code=@CurrencyCode,time_zone=@TimeZone,date_range_preference=@DateRangePreference,updated_at=@UpdatedAt,updated_by=@UpdatedBy WHERE user_id=@UserId AND deleted_at IS NULL;
            ELSE
              INSERT INTO user_preferences(id,user_id,currency_code,time_zone,date_range_preference,created_at,created_by,updated_at,updated_by,deleted_at,deleted_by)
              VALUES(@Id,@UserId,@CurrencyCode,@TimeZone,@DateRangePreference,@CreatedAt,@CreatedBy,@UpdatedAt,@UpdatedBy,@DeletedAt,@DeletedBy);
            """;
        await using var c=factory.CreateConnection(); await c.ExecuteAsync(new CommandDefinition(sql,x,cancellationToken:ct)); return x;
    }
}
