using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public sealed class Household : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
}
