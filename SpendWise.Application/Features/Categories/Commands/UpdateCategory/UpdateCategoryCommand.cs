using MediatR;
using SpendWise.Application.Features.Categories.DTOs;

namespace SpendWise.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid UserId, Guid HouseholdId, Guid CategoryId, string Name, string? Icon, string? Color, bool IsActive) : IRequest<CategoryDto>;
