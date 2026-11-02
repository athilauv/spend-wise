namespace SpendWise.Application.Features.UserFriends.DTOs;

public sealed record UserFriendDto(Guid Id, Guid UserId, Guid FriendUserId, string FriendName, string FriendEmail, string Status, DateTimeOffset CreatedAt);
