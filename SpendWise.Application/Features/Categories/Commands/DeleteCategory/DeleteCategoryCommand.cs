using MediatR;

namespace SpendWise.Application.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid UserId, Guid HouseholdId, Guid CategoryId) : IRequest<bool>;
