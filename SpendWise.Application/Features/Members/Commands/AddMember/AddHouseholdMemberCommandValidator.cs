using FluentValidation;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.AddMember;

public sealed class AddHouseholdMemberCommandValidator : AbstractValidator<AddHouseholdMemberCommand>
{
    public AddHouseholdMemberCommandValidator()
    {
        RuleFor(x => x.RequestingUserId).NotEmpty();
        RuleFor(x => x.HouseholdId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum().Must(role => role != MemberRole.Owner).WithMessage("Owner role is assigned only when creating a household.");
    }
}
