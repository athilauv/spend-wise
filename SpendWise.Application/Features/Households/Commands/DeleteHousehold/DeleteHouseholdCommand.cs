using MediatR;

namespace SpendWise.Application.Features.Households.Commands.DeleteHousehold;

public sealed record DeleteHouseholdCommand(Guid UserId, Guid HouseholdId) : IRequest<bool>;
