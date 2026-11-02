using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Categories.DTOs;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository repository, IHouseholdRepository households)
    : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var role = await households.GetMemberRoleAsync(request.HouseholdId, request.UserId, cancellationToken);
        if (role is null) throw new NotFoundException("Household not found or user is not a member.");
        var entity = await repository.GetByIdAsync(request.CategoryId, request.UserId, request.HouseholdId, cancellationToken)
            ?? throw new NotFoundException("Category not found.");
        if (entity.IsSystem) throw new InvalidOperationException("System categories cannot be modified.");
        if (entity.CreatedBy != request.UserId && role is not (MemberRole.Owner or MemberRole.Admin))
            throw new UnauthorizedAccessException("You are not allowed to modify this category.");
        if (await repository.NameExistsAsync(request.Name.Trim(), request.UserId, request.HouseholdId, request.CategoryId, cancellationToken))
            throw new InvalidOperationException("A category with this name already exists in this household.");

        entity.Name = request.Name.Trim();
        entity.Icon = request.Icon?.Trim();
        entity.Color = request.Color?.Trim();
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedBy = request.UserId;
        if (!await repository.UpdateAsync(entity, cancellationToken)) throw new NotFoundException("Category not found.");
        return new(entity.Id, entity.Name, entity.Icon, entity.Color, entity.IsSystem, entity.IsActive, entity.UserId, entity.HouseholdId);
    }
}
