using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Categories.DTOs;

namespace SpendWise.Application.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(ICategoryRepository repository, IHouseholdRepository households)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (await households.GetMemberRoleAsync(request.HouseholdId, request.UserId, cancellationToken) is null)
            throw new NotFoundException("Household not found or user is not a member.");
        var items = await repository.GetForUserAsync(request.UserId, request.HouseholdId, cancellationToken);
        return items.Select(x => new CategoryDto(x.Id, x.Name, x.Icon, x.Color, x.IsSystem, x.IsActive, x.UserId, x.HouseholdId)).ToArray();
    }
}
