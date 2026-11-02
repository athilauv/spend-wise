namespace SpendWise.Application.Abstractions.Authentication;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
