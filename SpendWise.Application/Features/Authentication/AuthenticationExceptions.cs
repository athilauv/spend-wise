namespace SpendWise.Application.Features.Authentication;
public sealed class DuplicateEmailException() : Exception("An account with this email already exists.");
public sealed class InvalidCredentialsException() : Exception("Email or password is incorrect.");
