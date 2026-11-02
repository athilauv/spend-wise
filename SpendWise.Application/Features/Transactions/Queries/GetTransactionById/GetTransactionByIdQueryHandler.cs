using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Transactions.Commands.CreateTransaction;
using SpendWise.Application.Features.Transactions.DTOs;

namespace SpendWise.Application.Features.Transactions.Queries.GetTransactionById;

public sealed class GetTransactionByIdQueryHandler(ITransactionRepository repository) : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    public async Task<TransactionDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        // Only household members may read transactions in their household.
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("You are not a member of this household.");
        var entity = await repository.GetByIdAsync(request.TransactionId, request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new NotFoundException("Transaction not found.");
        return CreateTransactionCommandHandler.Map(entity);
    }
}
