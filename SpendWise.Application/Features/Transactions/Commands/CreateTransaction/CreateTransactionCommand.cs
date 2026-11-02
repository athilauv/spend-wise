using MediatR;
using SpendWise.Application.Features.Transactions.DTOs;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Transactions.Commands.CreateTransaction;

public sealed record CreateTransactionCommand(
    Guid UserId, Guid HouseholdId, Guid PaidByUserId, Guid? CategoryId, TransactionType Type,
    decimal Amount, string? ShopName, string? Description, DateTimeOffset TransactionAt,
    string? PaymentMethod, string? Notes, BeneficiaryType BeneficiaryType, Guid? BeneficiaryUserId) : IRequest<TransactionDto>;
