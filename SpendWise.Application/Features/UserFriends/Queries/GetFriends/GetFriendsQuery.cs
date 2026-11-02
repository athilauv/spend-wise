using MediatR;
using SpendWise.Application.Features.UserFriends.DTOs;

namespace SpendWise.Application.Features.UserFriends.Queries.GetFriends;

public sealed record GetFriendsQuery(Guid UserId) : IRequest<IReadOnlyList<UserFriendDto>>;
