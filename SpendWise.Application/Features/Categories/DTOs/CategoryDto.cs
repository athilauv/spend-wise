namespace SpendWise.Application.Features.Categories.DTOs;

public sealed record CategoryDto(
    Guid Id, string Name, string? Icon, string? Color, bool IsSystem, bool IsActive,
    Guid? UserId, Guid? HouseholdId);
