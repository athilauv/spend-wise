using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.Authentication.Commands.Login;
using SpendWise.Application.Features.Authentication.Commands.Register;
using SpendWise.Application.Features.Authentication.Commands.Refresh;
using SpendWise.Application.Features.Authentication.DTOs;
namespace SpendWise.Api.Controllers;
[ApiController, Route("api/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
        => StatusCode(StatusCodes.Status201Created, await sender.Send(new RegisterCommand(request.FullName, request.Email, request.Password), ct));
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        => Ok(await sender.Send(new LoginCommand(request.Email, request.Password), ct));
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken ct)
        => Ok(await sender.Send(new RefreshCommand(request.RefreshToken), ct));
}
public sealed record RefreshRequest(string RefreshToken);
