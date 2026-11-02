using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.Analytics;
namespace SpendWise.Api.Controllers;
[ApiController, Authorize, Route("api/analytics")]
public sealed class AnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid householdId, [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to, [FromQuery] string groupBy = "month", CancellationToken ct = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await sender.Send(new GetAnalyticsQuery(householdId, userId, from, to, groupBy), ct));
    }
}
