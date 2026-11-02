using MediatR;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SpendWise.Application.Abstractions.Authentication;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Authentication;
using SpendWise.Application.Features.Authentication.DTOs;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Authentication.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly int _expiryMinutes;
    private readonly int _refreshDays;
    private readonly IRefreshTokenRepository _refreshTokens;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokens,
        IOptions<JwtTokenOptions> options)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokens = refreshTokens;
        _expiryMinutes = options.Value.AccessTokenExpirationMinutes;
        _refreshDays = options.Value.RefreshTokenExpirationDays;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var fullName = request.FullName.Trim();

        if (await _userRepository.EmailExistsAsync(email))
            throw new DuplicateEmailException();

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var now = DateTimeOffset.UtcNow;
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var token = new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id,
            TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))),
            ExpiresAt = now.AddDays(_refreshDays), CreatedAt = now, CreatedBy = user.Id };
        await _refreshTokens.SaveAsync(token, cancellationToken);
        return new AuthResponse("Successfully registered.", _jwtTokenService.GenerateAccessToken(user.Id, user.Email),
            raw, DateTime.UtcNow.AddMinutes(_expiryMinutes));
    }
}
