using FluentValidation;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Members.Commands.UpdateMemberRole;

public sealed class UpdateHouseholdMemberRoleCommandValidator : AbstractValidator<UpdateHouseholdMemberRoleCommand>
{
    public UpdateHouseholdMemberRoleCommandValidator()
    {
        RuleFor(x => x.RequestingUserId).NotEmpty();
        RuleFor(x => x.HouseholdId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum().Must(x => x != MemberRole.Owner).WithMessage("Owner role cannot be assigned through this operation.");
    }
}
