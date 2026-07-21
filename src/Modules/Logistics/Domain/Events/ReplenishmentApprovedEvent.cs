using PIMS_MS.Modules.Logistics.Domain.Entities;

namespace PIMS_MS.Modules.Logistics.Domain.Events;
public record ReplenishmentApprovedEvent(
    Guid ReplenishmentId, 
    Guid LocationId, 
    IReadOnlyCollection<ReplenishmentItem> Items
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}