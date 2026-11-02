using MediatR;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Features.Transactions.DTOs;

namespace SpendWise.Application.Features.Transactions.Queries.GetTransactions;

public sealed record GetTransactionsQuery(
    Guid UserId, Guid HouseholdId, DateTimeOffset? From, DateTimeOffset? To,
    Guid? CategoryId, string? Type, int Page = 1, int PageSize = 20) : IRequest<PagedResult<TransactionDto>>;
