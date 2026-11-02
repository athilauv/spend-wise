using MediatR;
using SpendWise.Application.Features.Authentication.DTOs;

namespace SpendWise.Application.Features.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password
) : IRequest<AuthResponse>;