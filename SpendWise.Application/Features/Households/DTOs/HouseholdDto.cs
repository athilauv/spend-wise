namespace SpendWise.Application.Features.Households.DTOs;

public sealed record HouseholdDto(Guid Id, string Name, Guid OwnerUserId, DateTimeOffset CreatedAt);
