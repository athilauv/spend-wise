using FluentValidation;

namespace SpendWise.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.HouseholdId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Icon).MaximumLength(60);
        RuleFor(x => x.Color).MaximumLength(20);
    }
}
