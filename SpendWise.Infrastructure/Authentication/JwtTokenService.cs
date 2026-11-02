using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpendWise.Application.Abstractions.Authentication;

namespace SpendWise.Infrastructure.Authentication;

// Generates signed JWT access tokens for authenticated users.
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;

        // Fail early if the JWT configuration is missing.
        if (string.IsNullOrWhiteSpace(_settings.Issuer))
            throw new InvalidOperationException("JWT issuer is not configured.");

        if (string.IsNullOrWhiteSpace(_settings.Audience))
            throw new InvalidOperationException("JWT audience is not configured.");

        if (Encoding.UTF8.GetByteCount(_settings.SecretKey) < 32)
            throw new InvalidOperationException("JWT secret key must contain at least 32 bytes.");

        if (_settings.AccessTokenExpirationMinutes <= 0)
            throw new InvalidOperationException("JWT expiration must be greater than zero.");
    }

    public string GenerateAccessToken(Guid userId, string email)
    {
        // Identify the user represented by this token.
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Create a signing key from the configured secret.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        // Set the issuer, audience, and token expiration.
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}