namespace SpendWise.Application.Features.UserPreferences.DTOs;

public sealed record UserPreferenceDto(Guid Id, Guid UserId, string CurrencyCode, string TimeZone, string? DateRangePreference);
