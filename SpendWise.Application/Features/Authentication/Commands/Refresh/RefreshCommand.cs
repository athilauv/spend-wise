using MediatR;
using SpendWise.Application.Features.Authentication.DTOs;
namespace SpendWise.Application.Features.Authentication.Commands.Refresh;
public sealed record RefreshCommand(string RefreshToken) : IRequest<AuthResponse>;
