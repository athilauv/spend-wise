using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Features.Transactions.Commands.CreateTransaction;
using SpendWise.Application.Features.Transactions.Commands.DeleteTransaction;
using SpendWise.Application.Features.Transactions.Commands.UpdateTransaction;
using SpendWise.Application.Features.Transactions.Queries.GetTransactionById;
using SpendWise.Application.Features.Transactions.Queries.GetTransactions;
using SpendWise.Domain.Enums;

namespace SpendWise.Api.Controllers;

[ApiController, Authorize, Route("api/transactions")]
public sealed class TransactionsController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid householdId, [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to, [FromQuery] Guid? categoryId,
        [FromQuery] string? type, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await sender.Send(new GetTransactionsQuery(CurrentUserId, householdId, from, to, categoryId, type, page, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, [FromQuery] Guid householdId, CancellationToken ct)
        => Ok(await sender.Send(new GetTransactionByIdQuery(CurrentUserId, householdId, id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionBody body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateTransactionCommand(
            CurrentUserId, body.HouseholdId, body.PaidByUserId, body.CategoryId, body.Type,
            body.Amount, body.ShopName, body.Description, body.TransactionAt,
            body.PaymentMethod, body.Notes, body.BeneficiaryType, body.BeneficiaryUserId), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id, householdId = result.HouseholdId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTransactionBody body, CancellationToken ct)
        => Ok(await sender.Send(new UpdateTransactionCommand(
            CurrentUserId,
            body.HouseholdId,
            id,
            body.PaidByUserId,
            body.CategoryId,
            body.Type,
            body.Amount,
            body.ShopName,
            body.Description,
            body.TransactionAt,
            body.PaymentMethod,
            body.Notes,
            body.BeneficiaryType,
            body.BeneficiaryUserId), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid householdId, CancellationToken ct)
    {
        await sender.Send(new DeleteTransactionCommand(CurrentUserId, householdId, id), ct);
        return NoContent();
    }
}

public sealed record CreateTransactionBody(
    Guid HouseholdId, Guid PaidByUserId, Guid? CategoryId, TransactionType Type, decimal Amount,
    string? ShopName, string? Description, DateTimeOffset TransactionAt, string? PaymentMethod,
    string? Notes, BeneficiaryType BeneficiaryType = BeneficiaryType.All, Guid? BeneficiaryUserId = null);

public sealed record UpdateTransactionBody(
    Guid HouseholdId, Guid PaidByUserId, Guid? CategoryId, TransactionType Type, decimal Amount,
    string? ShopName, string? Description, DateTimeOffset TransactionAt, string? PaymentMethod,
    string? Notes, BeneficiaryType BeneficiaryType = BeneficiaryType.All, Guid? BeneficiaryUserId = null);
