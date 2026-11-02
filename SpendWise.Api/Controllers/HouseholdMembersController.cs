using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.Members.Commands.AddMember;
using SpendWise.Application.Features.Members.Commands.RemoveMember;
using SpendWise.Application.Features.Members.Commands.UpdateMemberRole;
using SpendWise.Application.Features.Members.Queries.GetHouseholdMembers;
using SpendWise.Domain.Enums;

namespace SpendWise.Api.Controllers;

[ApiController, Authorize, Route("api/households/{householdId:guid}/members")]
public sealed class HouseholdMembersController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> List(Guid householdId, CancellationToken ct)
        => Ok(await sender.Send(new GetHouseholdMembersQuery(CurrentUserId, householdId), ct));

    [HttpPost]
    public async Task<IActionResult> Add(Guid householdId, AddHouseholdMemberBody body, CancellationToken ct)
    {
        var id = await sender.Send(new AddHouseholdMemberCommand(CurrentUserId, householdId, body.UserId, body.Role), ct);
        return StatusCode(StatusCodes.Status201Created, new { memberId = id });
    }

    [HttpPut("{memberId:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid householdId, Guid memberId, UpdateHouseholdMemberRoleBody body, CancellationToken ct)
        => Ok(new { updated = await sender.Send(new UpdateHouseholdMemberRoleCommand(CurrentUserId, householdId, memberId, body.Role), ct) });

    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> Remove(Guid householdId, Guid memberId, CancellationToken ct)
    {
        await sender.Send(new RemoveHouseholdMemberCommand(CurrentUserId, householdId, memberId), ct);
        return NoContent();
    }
}

public sealed record AddHouseholdMemberBody(Guid UserId, MemberRole Role = MemberRole.Member);
public sealed record UpdateHouseholdMemberRoleBody(MemberRole Role);
