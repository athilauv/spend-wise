using MediatR;
using SpendWise.Application.Features.Authentication.DTOs;
namespace SpendWise.Application.Features.Authentication.Commands.Login;
public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
