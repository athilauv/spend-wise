using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Queries.GetHouseholdById;

public sealed class GetHouseholdByIdQueryHandler(IHouseholdRepository repository) : IRequestHandler<GetHouseholdByIdQuery, HouseholdDto>
{
    public async Task<HouseholdDto> Handle(GetHouseholdByIdQuery request, CancellationToken cancellationToken)
    {
        var x = await repository.GetByIdForMemberAsync(request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new NotFoundException("Household not found or user is not a member.");
        return new(x.Id, x.Name, x.OwnerUserId, x.CreatedAt);
    }
}
