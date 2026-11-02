using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.UserPreferences.DTOs;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.UserPreferences.Commands.UpdateUserPreference;

public sealed class UpdateUserPreferenceCommandHandler(IUserPreferenceRepository repository) : IRequestHandler<UpdateUserPreferenceCommand, UserPreferenceDto>
{
    public async Task<UserPreferenceDto> Handle(UpdateUserPreferenceCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var entity = current ?? new UserPreference { Id = Guid.NewGuid(), UserId = request.UserId, CreatedAt = now, CreatedBy = request.UserId };
        entity.CurrencyCode = request.CurrencyCode.ToUpperInvariant();
        entity.TimeZone = request.TimeZone.Trim();
        entity.DateRangePreference = request.DateRangePreference?.Trim();
        entity.UpdatedAt = current is null ? null : now;
        entity.UpdatedBy = current is null ? null : request.UserId;
        var saved = await repository.UpsertAsync(entity, cancellationToken);
        return new(saved.Id, saved.UserId, saved.CurrencyCode, saved.TimeZone, saved.DateRangePreference);
    }
}
