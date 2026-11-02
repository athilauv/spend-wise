using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Queries.GetHouseholds;

public sealed class GetHouseholdsQueryHandler(IHouseholdRepository repository) : IRequestHandler<GetHouseholdsQuery, IReadOnlyList<HouseholdDto>>
{
    public async Task<IReadOnlyList<HouseholdDto>> Handle(GetHouseholdsQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetForUserAsync(request.UserId, cancellationToken);
        return items.Select(x => new HouseholdDto(x.Id, x.Name, x.OwnerUserId, x.CreatedAt)).ToArray();
    }
}
