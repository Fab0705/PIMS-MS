namespace PIMS_MS.Modules.Inventory.Domain.Events;
public record WorkOrderCompletedEvent(Guid WorkOrderId, Guid LocationId) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}