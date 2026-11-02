using FluentValidation;

namespace SpendWise.Application.Features.Households.Commands.CreateHousehold;

public sealed class CreateHouseholdCommandValidator : AbstractValidator<CreateHouseholdCommand>
{
    public CreateHouseholdCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
