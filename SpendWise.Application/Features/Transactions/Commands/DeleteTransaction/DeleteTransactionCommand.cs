using MediatR;

namespace SpendWise.Application.Features.Transactions.Commands.DeleteTransaction;

public sealed record DeleteTransactionCommand(Guid UserId, Guid HouseholdId, Guid TransactionId) : IRequest<bool>;
