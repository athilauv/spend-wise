using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.Households.Commands.CreateHousehold;
using SpendWise.Application.Features.Households.Commands.DeleteHousehold;
using SpendWise.Application.Features.Households.Commands.UpdateHousehold;
using SpendWise.Application.Features.Households.Queries.GetHouseholdById;
using SpendWise.Application.Features.Households.Queries.GetHouseholds;

namespace SpendWise.Api.Controllers;

[ApiController, Authorize, Route("api/households")]
public sealed class HouseholdsController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => Ok(await sender.Send(new GetHouseholdsQuery(CurrentUserId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetHouseholdByIdQuery(CurrentUserId, id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateHouseholdBody body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateHouseholdCommand(CurrentUserId, body.Name), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateHouseholdBody body, CancellationToken ct)
        => Ok(await sender.Send(new UpdateHouseholdCommand(CurrentUserId, id, body.Name), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteHouseholdCommand(CurrentUserId, id), ct);
        return NoContent();
    }
}

public sealed record CreateHouseholdBody(string Name);
public sealed record UpdateHouseholdBody(string Name);
