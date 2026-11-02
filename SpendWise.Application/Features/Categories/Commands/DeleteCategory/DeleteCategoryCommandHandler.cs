using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(ICategoryRepository repository, IHouseholdRepository households)
    : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var role = await households.GetMemberRoleAsync(request.HouseholdId, request.UserId, cancellationToken);
        if (role is null) throw new NotFoundException("Household not found or user is not a member.");
        var category = await repository.GetByIdAsync(request.CategoryId, request.UserId, request.HouseholdId, cancellationToken)
            ?? throw new NotFoundException("Category not found.");
        if (category.IsSystem) throw new InvalidOperationException("System categories cannot be deleted.");
        if (category.CreatedBy != request.UserId && role is not (MemberRole.Owner or MemberRole.Admin))
            throw new UnauthorizedAccessException("You are not allowed to delete this category.");
        var deleted = await repository.SoftDeleteAsync(request.CategoryId, request.UserId, request.HouseholdId, cancellationToken);
        if (!deleted) throw new NotFoundException("Category not found.");
        return true;
    }
}
