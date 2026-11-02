using MediatR;
using SpendWise.Application.Features.Categories.DTOs;

namespace SpendWise.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid UserId, Guid HouseholdId, Guid CategoryId) : IRequest<CategoryDto>;
