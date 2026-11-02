using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.UserPreferences.Commands.UpdateUserPreference;
using SpendWise.Application.Features.UserPreferences.Queries.GetUserPreference;

namespace SpendWise.Api.Controllers;

[ApiController, Authorize, Route("api/user-preferences")]
public sealed class UserPreferencesController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await sender.Send(new GetUserPreferenceQuery(CurrentUserId), ct));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserPreferenceBody body, CancellationToken ct)
        => Ok(await sender.Send(new UpdateUserPreferenceCommand(CurrentUserId, body.CurrencyCode, body.TimeZone, body.DateRangePreference), ct));
}

public sealed record UpdateUserPreferenceBody(string CurrencyCode, string TimeZone, string? DateRangePreference);
