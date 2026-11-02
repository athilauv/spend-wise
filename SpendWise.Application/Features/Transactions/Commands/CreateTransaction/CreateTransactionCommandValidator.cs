using FluentValidation;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.HouseholdId).NotEmpty();
        RuleFor(x => x.PaidByUserId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, false);
        RuleFor(x => x.ShopName).MaximumLength(160);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.BeneficiaryType).IsInEnum();
        RuleFor(x => x.BeneficiaryUserId).NotEmpty().When(x => x.BeneficiaryType == BeneficiaryType.Individual);
    }
}
