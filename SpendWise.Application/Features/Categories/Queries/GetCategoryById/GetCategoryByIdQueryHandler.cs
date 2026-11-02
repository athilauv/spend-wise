using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Categories.DTOs;

namespace SpendWise.Application.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(ICategoryRepository repository, IHouseholdRepository households)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (await households.GetMemberRoleAsync(request.HouseholdId, request.UserId, cancellationToken) is null)
            throw new NotFoundException("Household not found or user is not a member.");
        var x = await repository.GetByIdAsync(request.CategoryId, request.UserId, request.HouseholdId, cancellationToken)
            ?? throw new NotFoundException("Category not found.");
        return new(x.Id, x.Name, x.Icon, x.Color, x.IsSystem, x.IsActive, x.UserId, x.HouseholdId);
    }
}
