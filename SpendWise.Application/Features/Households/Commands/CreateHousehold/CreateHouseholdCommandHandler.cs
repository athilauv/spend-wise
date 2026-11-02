using MediatR;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Application.Features.Households.DTOs;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Households.Commands.CreateHousehold;

public sealed class CreateHouseholdCommandHandler(IHouseholdRepository repository) : IRequestHandler<CreateHouseholdCommand, HouseholdDto>
{
    public async Task<HouseholdDto> Handle(CreateHouseholdCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var household = new Household { Id = Guid.NewGuid(), Name = request.Name.Trim(), OwnerUserId = request.UserId, CreatedAt = now, CreatedBy = request.UserId };
        var membership = new HouseholdMember { Id = Guid.NewGuid(), HouseholdId = household.Id, UserId = request.UserId, Role = MemberRole.Owner, JoinedAt = now, IsActive = true, CreatedAt = now, CreatedBy = request.UserId };
        var saved = await repository.CreateWithOwnerAsync(household, membership, cancellationToken);
        return new(saved.Id, saved.Name, saved.OwnerUserId, saved.CreatedAt);
    }
}
