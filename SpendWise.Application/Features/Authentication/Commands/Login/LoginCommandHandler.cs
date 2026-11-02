using MediatR;
using System.Security.Cryptography;
using System.Text;
using SpendWise.Domain.Entities;
using Microsoft.Extensions.Options;
using SpendWise.Application.Abstractions.Authentication;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Authentication.DTOs;
namespace SpendWise.Application.Features.Authentication.Commands.Login;
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasherService _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly int _expiryMinutes;
    private readonly int _refreshDays;
    private readonly IRefreshTokenRepository _refreshTokens;
    public LoginCommandHandler(IUserRepository users, IPasswordHasherService hasher,
        IJwtTokenService jwt, IRefreshTokenRepository refreshTokens, IOptions<SpendWise.Application.Features.Authentication.JwtTokenOptions> options)
    {
        _users = users; _hasher = hasher; _jwt = jwt; _refreshTokens = refreshTokens;
        _expiryMinutes = options.Value.AccessTokenExpirationMinutes;
        _refreshDays = options.Value.RefreshTokenExpirationDays;
    }
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, cancellationToken);
        if (user is null || !user.IsActive || !_hasher.VerifyPassword(user.PasswordHash, request.Password))
            throw new SpendWise.Application.Features.Authentication.InvalidCredentialsException();

        var now = DateTime.UtcNow;
        await _users.UpdateLastLoginAsync(user.Id, now, cancellationToken);
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var token = new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id,
            TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))),
            ExpiresAt = now.AddDays(_refreshDays), CreatedAt = now, CreatedBy = user.Id };
        await _refreshTokens.SaveAsync(token, cancellationToken);
        return new AuthResponse("Successfully logged in.", _jwt.GenerateAccessToken(user.Id, user.Email), raw, now.AddMinutes(_expiryMinutes));
    }
}
