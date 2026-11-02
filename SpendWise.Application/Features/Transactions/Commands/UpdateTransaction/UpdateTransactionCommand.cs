using MediatR;
using SpendWise.Application.Features.Transactions.DTOs;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Transactions.Commands.UpdateTransaction;

public sealed record UpdateTransactionCommand(
    Guid UserId, Guid HouseholdId, Guid TransactionId, Guid PaidByUserId, Guid? CategoryId, TransactionType Type,
    decimal Amount, string? ShopName, string? Description, DateTimeOffset TransactionAt,
    string? PaymentMethod, string? Notes, BeneficiaryType BeneficiaryType, Guid? BeneficiaryUserId) : IRequest<TransactionDto>;
