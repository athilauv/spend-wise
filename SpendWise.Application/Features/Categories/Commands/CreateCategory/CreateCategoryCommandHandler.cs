using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Categories.DTOs;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(ICategoryRepository repository, IHouseholdRepository households)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await households.GetMemberRoleAsync(request.HouseholdId, request.UserId, cancellationToken) is null)
            throw new NotFoundException("Household not found or user is not a member.");
        if (await repository.NameExistsAsync(request.Name.Trim(), request.UserId, request.HouseholdId, null, cancellationToken))
            throw new InvalidOperationException("A category with this name already exists in this household.");

        var entity = new Category
        {
            Id = Guid.NewGuid(), UserId = null, HouseholdId = request.HouseholdId,
            Name = request.Name.Trim(), Icon = request.Icon?.Trim(), Color = request.Color?.Trim(),
            IsSystem = false, IsActive = true, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = request.UserId
        };
        var saved = await repository.CreateAsync(entity, cancellationToken);
        return new(saved.Id, saved.Name, saved.Icon, saved.Color, saved.IsSystem, saved.IsActive, saved.UserId, saved.HouseholdId);
    }
}
