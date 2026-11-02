using MediatR;
using SpendWise.Application.Features.Categories.DTOs;

namespace SpendWise.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(Guid UserId, Guid HouseholdId, string Name, string? Icon, string? Color) : IRequest<CategoryDto>;
