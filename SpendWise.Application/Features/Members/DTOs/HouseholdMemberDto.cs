namespace SpendWise.Application.Features.Members.DTOs;

public sealed record HouseholdMemberDto(Guid Id, Guid HouseholdId, Guid UserId, string FullName, string Email, string Role, DateTimeOffset JoinedAt, bool IsActive);
