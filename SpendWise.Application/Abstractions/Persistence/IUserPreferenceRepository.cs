using SpendWise.Domain.Entities;

namespace SpendWise.Application.Abstractions.Persistence;

public interface IUserPreferenceRepository
{
    Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserPreference> UpsertAsync(UserPreference preference, CancellationToken cancellationToken);
}
