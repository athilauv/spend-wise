using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Features.Transactions.Commands.CreateTransaction;
using SpendWise.Application.Features.Transactions.DTOs;

namespace SpendWise.Application.Features.Transactions.Queries.GetTransactions;

public sealed class GetTransactionsQueryHandler(ITransactionRepository repository) : IRequestHandler<GetTransactionsQuery, PagedResult<TransactionDto>>
{
    public async Task<PagedResult<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("You are not a member of this household.");
        var filter = new TransactionFilter(request.HouseholdId, request.UserId, request.From, request.To, request.CategoryId, request.Type, request.Page, request.PageSize);
        var (items, total) = await repository.GetPagedAsync(filter, cancellationToken);
        return new PagedResult<TransactionDto>(items.Select(CreateTransactionCommandHandler.Map).ToArray(), total, request.Page, request.PageSize);
    }
}
