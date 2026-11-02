namespace SpendWise.Application.Features.Authentication.DTOs;
public sealed record AuthResponse(string Message, string AccessToken, string RefreshToken, DateTime ExpiresAt);
