namespace PIMS_MS.Modules.Inventory.Domain.Events;

public record WorkOrderCreatedEvent(Guid WorkOrderId, Guid LocationId, string WorkOrderNumber) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}