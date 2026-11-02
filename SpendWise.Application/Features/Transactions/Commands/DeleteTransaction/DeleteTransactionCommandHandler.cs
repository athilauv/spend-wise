using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.Transactions.Commands.DeleteTransaction;

public sealed class DeleteTransactionCommandHandler(ITransactionRepository repository) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("You are not a member of this household.");
        var entity = await repository.GetByIdAsync(request.TransactionId, request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new NotFoundException("Transaction not found.");
        if (entity.UserId != request.UserId)
            throw new UnauthorizedAccessException("Only the creator of this transaction can delete it.");
        if (!await repository.SoftDeleteAsync(request.TransactionId, request.HouseholdId, request.UserId, cancellationToken))
            throw new NotFoundException("Transaction not found.");
        return true;
    }
}
