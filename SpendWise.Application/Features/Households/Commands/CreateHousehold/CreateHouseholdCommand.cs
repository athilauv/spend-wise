using MediatR;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Commands.CreateHousehold;

public sealed record CreateHouseholdCommand(Guid UserId, string Name) : IRequest<HouseholdDto>;
