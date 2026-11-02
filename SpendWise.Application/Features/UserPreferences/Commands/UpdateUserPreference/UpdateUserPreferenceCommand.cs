using MediatR;
using SpendWise.Application.Features.UserPreferences.DTOs;

namespace SpendWise.Application.Features.UserPreferences.Commands.UpdateUserPreference;

public sealed record UpdateUserPreferenceCommand(Guid UserId, string CurrencyCode, string TimeZone, string? DateRangePreference) : IRequest<UserPreferenceDto>;
