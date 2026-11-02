using MediatR;
using SpendWise.Application.Features.UserPreferences.DTOs;

namespace SpendWise.Application.Features.UserPreferences.Queries.GetUserPreference;

public sealed record GetUserPreferenceQuery(Guid UserId) : IRequest<UserPreferenceDto>;
