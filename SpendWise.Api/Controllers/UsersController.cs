using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpendWise.Api.Controllers;

/// <summary>Current-user endpoint. Never accepts a user ID from the client for this route.</summary>
[ApiController, Authorize, Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (!Guid.TryParse(id, out var userId))
            throw new UnauthorizedAccessException("User ID claim is missing or invalid.");

        return Ok(new { userId, email });
    }
}
