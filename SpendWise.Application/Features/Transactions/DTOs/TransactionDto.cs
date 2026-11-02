namespace SpendWise.Application.Features.Transactions.DTOs;

public sealed record TransactionDto(
    Guid Id, Guid HouseholdId, Guid UserId, Guid PaidByUserId, Guid? CategoryId,
    string Type, decimal Amount, string? ShopName, string? Description,
    DateTimeOffset TransactionAt, string? PaymentMethod, string? Notes,
    string BeneficiaryType, Guid? BeneficiaryUserId);
