using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using SpendWise.Application.Abstractions.Authentication;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Authentication.DTOs;
using SpendWise.Domain.Entities;
namespace SpendWise.Application.Features.Authentication.Commands.Refresh;
public sealed class RefreshCommandHandler(IRefreshTokenRepository refreshTokens, IUserRepository users,
    IJwtTokenService jwt, IOptions<SpendWise.Application.Features.Authentication.JwtTokenOptions> options)
    : IRequestHandler<RefreshCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new SpendWise.Application.Features.Authentication.InvalidCredentialsException();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
        var current = await refreshTokens.GetByHashAsync(hash, ct);
        var now = DateTimeOffset.UtcNow;
        if (current is null || current.RevokedAt.HasValue || current.ExpiresAt <= now)
            throw new SpendWise.Application.Features.Authentication.InvalidCredentialsException();
        var user = await users.GetByIdAsync(current.UserId, ct);
        if (user is null || !user.IsActive)
            throw new SpendWise.Application.Features.Authentication.InvalidCredentialsException();
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var replacement = new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id,
            TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))),
            ExpiresAt = now.AddDays(options.Value.RefreshTokenExpirationDays), CreatedAt = now, CreatedBy = user.Id };
        await refreshTokens.SaveAsync(replacement, ct);
        await refreshTokens.RevokeAsync(current.Id, now, replacement.Id, ct);
        return new AuthResponse("Successfully refreshed.", jwt.GenerateAccessToken(user.Id, user.Email), raw,
            DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpirationMinutes));
    }
}
