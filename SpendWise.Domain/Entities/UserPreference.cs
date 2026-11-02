using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public sealed class UserPreference : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyCode { get; set; } = "AED";
    public string TimeZone { get; set; } = "Asia/Dubai";
    public string? DateRangePreference { get; set; }
}
