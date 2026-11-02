using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Application.Features.Members.DTOs;

namespace SpendWise.Application.Abstractions.Persistence;

public interface IHouseholdMemberRepository
{
    Task<IReadOnlyList<HouseholdMemberDto>> GetByHouseholdAsync(Guid householdId, CancellationToken cancellationToken);
    Task<HouseholdMember?> GetAsync(Guid householdId, Guid memberId, CancellationToken cancellationToken);
    Task<bool> IsMemberAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> AddAsync(HouseholdMember member, CancellationToken cancellationToken);
    Task<bool> UpdateRoleAsync(Guid householdId, Guid memberId, MemberRole role, Guid updatedBy, CancellationToken cancellationToken);
    Task<bool> RemoveAsync(Guid householdId, Guid memberId, Guid deletedBy, CancellationToken cancellationToken);
    Task<MemberRole?> GetRoleAsync(Guid householdId, Guid userId, CancellationToken cancellationToken);
}
