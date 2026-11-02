using SpendWise.Domain.Entities;
namespace SpendWise.Application.Abstractions.Persistence;
public interface IRefreshTokenRepository
{
    Task SaveAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct);
    Task RevokeAsync(Guid id, DateTimeOffset revokedAt, Guid? replacedByTokenId, CancellationToken ct);
}
