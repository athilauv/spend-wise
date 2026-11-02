using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.UserFriends.Commands.RemoveFriend;
using SpendWise.Application.Features.UserFriends.Commands.RespondFriendRequest;
using SpendWise.Application.Features.UserFriends.Commands.SendFriendRequest;
using SpendWise.Application.Features.UserFriends.Queries.GetFriends;

namespace SpendWise.Api.Controllers;

[ApiController, Authorize, Route("api/friends")]
public sealed class UserFriendsController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => Ok(await sender.Send(new GetFriendsQuery(CurrentUserId), ct));

    [HttpPost("requests")]
    public async Task<IActionResult> SendRequest(SendFriendRequestBody body, CancellationToken ct)
    {
        var id = await sender.Send(new SendFriendRequestCommand(CurrentUserId, body.FriendUserId), ct);
        return StatusCode(StatusCodes.Status201Created, new { friendshipId = id });
    }

    [HttpPost("requests/{id:guid}/respond")]
    public async Task<IActionResult> Respond(Guid id, RespondFriendRequestBody body, CancellationToken ct)
        => Ok(new { updated = await sender.Send(new RespondFriendRequestCommand(CurrentUserId, id, body.Accept), ct) });

    [HttpDelete("requests/{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await sender.Send(new RemoveFriendCommand(CurrentUserId, id), ct);
        return NoContent();
    }
}

public sealed record SendFriendRequestBody(Guid FriendUserId);
public sealed record RespondFriendRequestBody(bool Accept);
