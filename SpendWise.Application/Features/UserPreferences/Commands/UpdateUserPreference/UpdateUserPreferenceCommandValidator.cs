using FluentValidation;

namespace SpendWise.Application.Features.UserPreferences.Commands.UpdateUserPreference;

public sealed class UpdateUserPreferenceCommandValidator : AbstractValidator<UpdateUserPreferenceCommand>
{
    public UpdateUserPreferenceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3).Matches("^[A-Za-z]{3}$");
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateRangePreference).MaximumLength(40);
    }
}
