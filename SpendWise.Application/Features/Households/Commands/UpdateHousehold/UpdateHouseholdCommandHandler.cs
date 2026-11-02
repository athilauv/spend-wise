using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Households.DTOs;

namespace SpendWise.Application.Features.Households.Commands.UpdateHousehold;

public sealed class UpdateHouseholdCommandHandler(IHouseholdRepository repository) : IRequestHandler<UpdateHouseholdCommand, HouseholdDto>
{
    public async Task<HouseholdDto> Handle(UpdateHouseholdCommand request, CancellationToken cancellationToken)
    {
        var household = await repository.GetByIdForMemberAsync(request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new NotFoundException("Household not found or user is not a member.");
        if (household.OwnerUserId != request.UserId) throw new UnauthorizedAccessException("Only the household owner can update its name.");
        household.Name = request.Name.Trim();
        household.UpdatedAt = DateTimeOffset.UtcNow;
        household.UpdatedBy = request.UserId;
        if (!await repository.UpdateAsync(household, cancellationToken)) throw new NotFoundException("Household not found.");
        return new(household.Id, household.Name, household.OwnerUserId, household.CreatedAt);
    }
}
