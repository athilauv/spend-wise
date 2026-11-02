using Microsoft.AspNetCore.Identity;
using SpendWise.Application.Abstractions.Authentication;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Authentication;

public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();
    public string HashPassword(string password)
    {
        return _hasher.HashPassword(new User(), password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(new User(), hashedPassword, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}