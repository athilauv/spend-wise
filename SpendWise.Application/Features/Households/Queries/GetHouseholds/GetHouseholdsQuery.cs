using MediatR;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Queries.GetHouseholds;

public sealed record GetHouseholdsQuery(Guid UserId) : IRequest<IReadOnlyList<HouseholdDto>>;
