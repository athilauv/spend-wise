using MediatR;
using SpendWise.Application.Features.Transactions.DTOs;

namespace SpendWise.Application.Features.Transactions.Queries.GetTransactionById;

public sealed record GetTransactionByIdQuery(Guid UserId, Guid HouseholdId, Guid TransactionId) : IRequest<TransactionDto>;
