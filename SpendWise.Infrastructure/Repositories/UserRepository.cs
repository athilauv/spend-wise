using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

// Handles database operations related to users.
public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Find an active, non-deleted user by email.
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                id AS Id,
                full_name AS FullName,
                email AS Email,
                password_hash AS PasswordHash,
                profile_photo_url AS ProfilePhotoUrl,
                is_active AS IsActive,
                last_login_at AS LastLoginAt,
                created_at AS CreatedAt,
                created_by AS CreatedBy,
                updated_at AS UpdatedAt,
                updated_by AS UpdatedBy,
                deleted_at AS DeletedAt,
                deleted_by AS DeletedBy
            FROM users
            WHERE LOWER(email) = LOWER(@Email)
              AND is_active = 1
              AND deleted_at IS NULL;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Email = email.Trim() }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    // Find an active, non-deleted user by ID.
    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                id AS Id,
                full_name AS FullName,
                email AS Email,
                password_hash AS PasswordHash,
                profile_photo_url AS ProfilePhotoUrl,
                is_active AS IsActive,
                last_login_at AS LastLoginAt,
                created_at AS CreatedAt,
                created_by AS CreatedBy,
                updated_at AS UpdatedAt,
                updated_by AS UpdatedBy,
                deleted_at AS DeletedAt,
                deleted_by AS DeletedBy
            FROM users
            WHERE id = @UserId
              AND is_active = 1
              AND deleted_at IS NULL;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    // Check whether an email is already registered.
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM users
                    WHERE LOWER(email) = LOWER(@Email)
                      AND deleted_at IS NULL
                )
                THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, new { Email = email.Trim() },
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    // Insert a new user into the database.
    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO users
            (
                id,
                full_name,
                email,
                password_hash,
                profile_photo_url,
                is_active,
                last_login_at,
                created_at,
                created_by,
                updated_at,
                updated_by,
                deleted_at,
                deleted_by
            )
            VALUES
            (
                @Id,
                @FullName,
                @Email,
                @PasswordHash,
                @ProfilePhotoUrl,
                @IsActive,
                @LastLoginAt,
                @CreatedAt,
                @CreatedBy,
                @UpdatedAt,
                @UpdatedBy,
                @DeletedAt,
                @DeletedBy
            );
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                user.Id,
                user.FullName,
                Email = user.Email.Trim(),
                user.PasswordHash,
                user.ProfilePhotoUrl,
                user.IsActive,
                user.LastLoginAt,
                user.CreatedAt,
                user.CreatedBy,
                user.UpdatedAt,
                user.UpdatedBy,
                user.DeletedAt,
                user.DeletedBy
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    // Record the latest successful login time.
    public async Task UpdateLastLoginAsync(
        Guid userId,
        DateTimeOffset loginAt,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE users
            SET last_login_at = @LoginAt,
                updated_at = @Now,
                updated_by = @UserId
            WHERE id = @UserId
              AND is_active = 1
              AND deleted_at IS NULL;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                LoginAt = loginAt,
                Now = DateTimeOffset.UtcNow
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}



