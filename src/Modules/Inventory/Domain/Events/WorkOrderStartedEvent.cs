using PIMS_MS.Modules.Inventory.Domain.Entities;

namespace PIMS_MS.Modules.Inventory.Domain.Events;

public record WorkOrderStartedEvent(Guid WorkOrderId, Guid LocationId, IReadOnlyCollection<WorkOrderItem> Items) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}