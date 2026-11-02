using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public sealed class UserFriend : AuditableEntity
{
    public const string Pending = "Pending";
    public const string Accepted = "Accepted";
    public const string Declined = "Declined";
    public const string Blocked = "Blocked";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FriendUserId { get; set; }
    public string Status { get; set; } = Pending; // Pending, Accepted, Declined, Blocked
    public DateTimeOffset? RespondedAt { get; set; }
}
