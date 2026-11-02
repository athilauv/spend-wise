using MediatR;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Commands.UpdateHousehold;

public sealed record UpdateHouseholdCommand(Guid UserId, Guid HouseholdId, string Name) : IRequest<HouseholdDto>;
