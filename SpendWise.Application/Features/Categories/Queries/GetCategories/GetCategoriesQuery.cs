using MediatR;
using SpendWise.Application.Features.Categories.DTOs;

namespace SpendWise.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery(Guid UserId, Guid HouseholdId) : IRequest<IReadOnlyList<CategoryDto>>;
