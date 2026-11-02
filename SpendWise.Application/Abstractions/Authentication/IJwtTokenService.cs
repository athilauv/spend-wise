namespace SpendWise.Application.Abstractions.Authentication;

// generate JWT access tokens
public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email);
}