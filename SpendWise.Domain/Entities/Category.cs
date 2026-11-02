using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public sealed class Category : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? HouseholdId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
}
