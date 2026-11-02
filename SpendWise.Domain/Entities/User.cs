using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public sealed class User : AuditableEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastLoginAt { get; set; }
}
