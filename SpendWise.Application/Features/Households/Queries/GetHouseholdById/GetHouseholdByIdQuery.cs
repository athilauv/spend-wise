using MediatR;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Queries.GetHouseholdById;

public sealed record GetHouseholdByIdQuery(Guid UserId, Guid HouseholdId) : IRequest<HouseholdDto>;
