namespace PIMS_MS.Modules.Inventory.Domain.Events;

public record StockDepletedEvent(Guid SparePartId, Guid LocationId, int CurrentQuantity) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}