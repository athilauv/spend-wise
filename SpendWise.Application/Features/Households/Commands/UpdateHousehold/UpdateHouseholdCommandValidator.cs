using FluentValidation;

namespace SpendWise.Application.Features.Households.Commands.UpdateHousehold;

public sealed class UpdateHouseholdCommandValidator : AbstractValidator<UpdateHouseholdCommand>
{
    public UpdateHouseholdCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.HouseholdId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
