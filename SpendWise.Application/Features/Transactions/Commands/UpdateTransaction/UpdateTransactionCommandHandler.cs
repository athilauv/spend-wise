using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Transactions.Commands.CreateTransaction;
using SpendWise.Application.Features.Transactions.DTOs;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Transactions.Commands.UpdateTransaction;

public sealed class UpdateTransactionCommandHandler(
    ITransactionRepository repository,
    IHouseholdRepository households) : IRequestHandler<UpdateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        // Verify caller is an active member of the household
        var callerRole = await households.GetMemberRoleAsync(request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("You are not a member of this household.");

        // Retrieve the transaction to update
        var entity = await repository.GetByIdAsync(request.TransactionId, request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new NotFoundException("Transaction not found.");

        // Allow updates only by the original creator, household Owner, or Admin
        if (entity.UserId != request.UserId && callerRole is not (MemberRole.Owner or MemberRole.Admin))
            throw new UnauthorizedAccessException("You do not have permission to modify this transaction.");

        // Validate paid-by user belongs to the household
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.PaidByUserId, cancellationToken))
            throw new InvalidOperationException("Paid-by user must be a household member.");

        // Validate beneficiary user belongs to the household when individual
        var beneficiaryUserId = request.BeneficiaryType == BeneficiaryType.Individual ? request.BeneficiaryUserId : null;
        if (beneficiaryUserId.HasValue && !await repository.IsHouseholdMemberAsync(request.HouseholdId, beneficiaryUserId.Value, cancellationToken))
            throw new InvalidOperationException("Beneficiary must be a household member.");

        // Validate category belongs to user/household scope
        if (request.CategoryId.HasValue && !await repository.CategoryBelongsToScopeAsync(request.CategoryId.Value, request.UserId, request.HouseholdId, cancellationToken))
            throw new InvalidOperationException("Category does not belong to this user or household.");

        // Update transaction details while preserving original creator and creation metadata
        entity.PaidByUserId = request.PaidByUserId;
        entity.CategoryId = request.CategoryId;
        entity.Type = request.Type;
        entity.Amount = request.Amount;
        entity.ShopName = request.ShopName?.Trim();
        entity.Description = request.Description?.Trim();
        entity.TransactionAt = request.TransactionAt;
        entity.PaymentMethod = request.PaymentMethod?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.BeneficiaryType = request.BeneficiaryType;
        entity.BeneficiaryUserId = beneficiaryUserId;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedBy = request.UserId;

        if (!await repository.UpdateAsync(entity, cancellationToken))
            throw new NotFoundException("Transaction not found.");

        return CreateTransactionCommandHandler.Map(entity);
    }
}

