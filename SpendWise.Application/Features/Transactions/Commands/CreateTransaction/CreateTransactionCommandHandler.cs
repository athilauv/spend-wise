using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Transactions.DTOs;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionCommandHandler(ITransactionRepository repository) : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.UserId, cancellationToken))
            throw new UnauthorizedAccessException("You are not a member of this household.");
        if (!await repository.IsHouseholdMemberAsync(request.HouseholdId, request.PaidByUserId, cancellationToken))
            throw new InvalidOperationException("Paid-by user must be a household member.");
        if (request.CategoryId.HasValue && !await repository.CategoryBelongsToScopeAsync(request.CategoryId.Value, request.UserId, request.HouseholdId, cancellationToken))
            throw new InvalidOperationException("Category does not belong to this user or household.");
        if (request.BeneficiaryUserId.HasValue && !await repository.IsHouseholdMemberAsync(request.HouseholdId, request.BeneficiaryUserId.Value, cancellationToken))
            throw new InvalidOperationException("Beneficiary must be a household member.");

        var entity = new SpendWise.Domain.Entities.Transaction
        {
            Id = Guid.NewGuid(), HouseholdId = request.HouseholdId, UserId = request.UserId, PaidByUserId = request.PaidByUserId,
            CategoryId = request.CategoryId, Type = request.Type, Amount = request.Amount, ShopName = request.ShopName?.Trim(),
            Description = request.Description?.Trim(), TransactionAt = request.TransactionAt, PaymentMethod = request.PaymentMethod?.Trim(),
            Notes = request.Notes?.Trim(), BeneficiaryType = request.BeneficiaryType, BeneficiaryUserId = request.BeneficiaryUserId,
            CreatedAt = DateTimeOffset.UtcNow, CreatedBy = request.UserId
        };
        var saved = await repository.CreateAsync(entity, cancellationToken);
        return Map(saved);
    }
    internal static TransactionDto Map(SpendWise.Domain.Entities.Transaction x) =>
        new(x.Id, x.HouseholdId, x.UserId, x.PaidByUserId, x.CategoryId, x.Type.ToString(), x.Amount, x.ShopName, x.Description, x.TransactionAt, x.PaymentMethod, x.Notes, x.BeneficiaryType.ToString(), x.BeneficiaryUserId);
}
