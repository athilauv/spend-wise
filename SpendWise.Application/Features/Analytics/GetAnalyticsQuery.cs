using MediatR;
using SpendWise.Application.Abstractions.Persistence;
namespace SpendWise.Application.Features.Analytics;
public sealed record GetAnalyticsQuery(Guid HouseholdId, Guid UserId, DateTimeOffset From, DateTimeOffset To, string GroupBy) : IRequest<TransactionAnalytics>;
public sealed class GetAnalyticsQueryHandler(ITransactionRepository repository) : IRequestHandler<GetAnalyticsQuery, TransactionAnalytics>
{
    public async Task<TransactionAnalytics> Handle(GetAnalyticsQuery request, CancellationToken ct)
    {
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.UserId, ct))
            throw new UnauthorizedAccessException("You are not a member of this household.");
        if (request.To <= request.From) throw new ArgumentException("'to' must be later than 'from'.");
        if (!new[] { "week", "month", "year" }.Contains(request.GroupBy.ToLowerInvariant()))
            throw new ArgumentException("groupBy must be week, month, or year.");
        return await repository.GetAnalyticsAsync(request.HouseholdId, request.From, request.To, request.GroupBy, ct);
    }
}
