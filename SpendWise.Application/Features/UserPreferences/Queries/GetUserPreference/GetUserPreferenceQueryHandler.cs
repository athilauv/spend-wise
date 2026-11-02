using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.UserPreferences.DTOs;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.UserPreferences.Queries.GetUserPreference;

public sealed class GetUserPreferenceQueryHandler(IUserPreferenceRepository repository) : IRequestHandler<GetUserPreferenceQuery, UserPreferenceDto>
{
    public async Task<UserPreferenceDto> Handle(GetUserPreferenceQuery request, CancellationToken cancellationToken)
    {
        var x = await repository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? new UserPreference { Id = Guid.Empty, UserId = request.UserId, CurrencyCode = "AED", TimeZone = "Asia/Dubai" };
        return new(x.Id, x.UserId, x.CurrencyCode, x.TimeZone, x.DateRangePreference);
    }
}
