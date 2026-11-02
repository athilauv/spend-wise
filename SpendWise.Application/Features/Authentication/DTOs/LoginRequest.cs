namespace SpendWise.Application.Features.Authentication.DTOs;

public sealed record LoginRequest(
    string Email,
    string Password
);