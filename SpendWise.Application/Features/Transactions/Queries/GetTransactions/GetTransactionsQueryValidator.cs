using FluentValidation;

namespace SpendWise.Application.Features.Transactions.Queries.GetTransactions;

public sealed class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
{
    public GetTransactionsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.HouseholdId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From!.Value).When(x => x.From.HasValue && x.To.HasValue);
        RuleFor(x => x.Type).Must(x => x is null || x.Equals("Income", StringComparison.OrdinalIgnoreCase) || x.Equals("Expense", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Type must be Income or Expense.");
    }
}
