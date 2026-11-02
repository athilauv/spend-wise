using SpendWise.Domain.Entities;

namespace SpendWise.Application.Abstractions.Persistence;

// Defines the database operations available for users.
public interface IUserRepository
{
    // Find an active user by email address.
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    // Find an active user by ID.
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    // Check whether an email address is already registered.
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    // Save a newly registered user.
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    // Update the user's last login timestamp.
    Task UpdateLastLoginAsync(Guid userId, DateTimeOffset loginAt, CancellationToken cancellationToken = default);
}