using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.Categories.Commands.CreateCategory;
using SpendWise.Application.Features.Categories.Commands.DeleteCategory;
using SpendWise.Application.Features.Categories.Commands.UpdateCategory;
using SpendWise.Application.Features.Categories.Queries.GetCategories;
using SpendWise.Application.Features.Categories.Queries.GetCategoryById;

namespace SpendWise.Api.Controllers;

[ApiController, Authorize, Route("api/categories")]
public sealed class CategoriesController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid householdId, CancellationToken ct)
        => Ok(await sender.Send(new GetCategoriesQuery(CurrentUserId, householdId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, [FromQuery] Guid householdId, CancellationToken ct)
        => Ok(await sender.Send(new GetCategoryByIdQuery(CurrentUserId, householdId, id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryBody body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCategoryCommand(CurrentUserId, body.HouseholdId, body.Name, body.Icon, body.Color), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id, householdId = body.HouseholdId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryBody body, CancellationToken ct)
        => Ok(await sender.Send(new UpdateCategoryCommand(CurrentUserId, body.HouseholdId, id, body.Name, body.Icon, body.Color, body.IsActive), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid householdId, CancellationToken ct)
    {
        await sender.Send(new DeleteCategoryCommand(CurrentUserId, householdId, id), ct);
        return NoContent();
    }
}

public sealed record CreateCategoryBody(Guid HouseholdId, string Name, string? Icon, string? Color);
public sealed record UpdateCategoryBody(Guid HouseholdId, string Name, string? Icon, string? Color, bool IsActive = true);
