using SpendWise.Domain.Entities;

namespace SpendWise.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetForUserAsync(Guid userId, Guid householdId, CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(Guid id, Guid userId, Guid householdId, CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(string name, Guid userId, Guid householdId, Guid? excludeId, CancellationToken cancellationToken);
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken);
    Task<bool> SoftDeleteAsync(Guid id, Guid userId, Guid householdId, CancellationToken cancellationToken);
}
