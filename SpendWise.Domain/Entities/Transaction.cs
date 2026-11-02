using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Entities;

public sealed class Transaction : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid UserId { get; set; }
    public Guid PaidByUserId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SuggestedCategoryId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? ShopName { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset TransactionAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public BeneficiaryType BeneficiaryType { get; set; } = BeneficiaryType.All;
    public Guid? BeneficiaryUserId { get; set; }
}
