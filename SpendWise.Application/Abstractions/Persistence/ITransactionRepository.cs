using SpendWise.Domain.Entities;
namespace SpendWise.Application.Abstractions.Persistence;
public sealed record TransactionFilter(Guid HouseholdId, Guid UserId, DateTimeOffset? From, DateTimeOffset? To, Guid? CategoryId, string? Type, int Page, int PageSize);
public sealed record AnalyticsPoint(string Period, decimal Income, decimal Expense);
public sealed record TransactionAnalytics(decimal TotalIncome, decimal TotalExpense, decimal Balance, IReadOnlyList<AnalyticsPoint> Series);
public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, Guid householdId, Guid userId, CancellationToken cancellationToken);
    Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedAsync(TransactionFilter filter, CancellationToken cancellationToken);
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<bool> SoftDeleteAsync(Guid id, Guid householdId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CategoryBelongsToScopeAsync(Guid categoryId, Guid userId, Guid householdId, CancellationToken cancellationToken);
    Task<bool> IsHouseholdMemberAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);
    Task<TransactionAnalytics> GetAnalyticsAsync(Guid householdId, DateTimeOffset from, DateTimeOffset to, string groupBy, CancellationToken cancellationToken);
}
