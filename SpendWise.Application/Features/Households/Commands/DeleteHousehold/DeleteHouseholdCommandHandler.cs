using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.Households.Commands.DeleteHousehold;

public sealed class DeleteHouseholdCommandHandler(IHouseholdRepository repository) : IRequestHandler<DeleteHouseholdCommand, bool>
{
    public async Task<bool> Handle(DeleteHouseholdCommand request, CancellationToken cancellationToken)
    {
        var household = await repository.GetByIdForMemberAsync(request.HouseholdId, request.UserId, cancellationToken)
            ?? throw new NotFoundException("Household not found or user is not a member.");
        if (household.OwnerUserId != request.UserId) throw new UnauthorizedAccessException("Only the owner can delete the household.");
        var deleted = await repository.SoftDeleteAsync(request.HouseholdId, request.UserId, cancellationToken);
        if (!deleted) throw new NotFoundException("Household not found.");
        return true;
    }
}
