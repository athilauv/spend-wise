using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Abstractions.Persistence;

public interface IHouseholdRepository
{
    Task<Household> CreateWithOwnerAsync(Household household, HouseholdMember ownerMembership, CancellationToken cancellationToken);
    Task<IReadOnlyList<Household>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Household?> GetByIdForMemberAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Household household, CancellationToken cancellationToken);
    Task<bool> SoftDeleteAsync(Guid householdId, Guid ownerUserId, CancellationToken cancellationToken);
    Task<MemberRole?> GetMemberRoleAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);
}
