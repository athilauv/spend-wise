using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Entities;

public sealed class HouseholdMember : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; } = MemberRole.Member;
    public DateTimeOffset JoinedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
