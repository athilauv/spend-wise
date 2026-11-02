using FluentValidation;

namespace SpendWise.Application.Features.UserFriends.Commands.SendFriendRequest;

public sealed class SendFriendRequestCommandValidator : AbstractValidator<SendFriendRequestCommand>
{
    public SendFriendRequestCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FriendUserId).NotEmpty().NotEqual(x => x.UserId);
    }
}
