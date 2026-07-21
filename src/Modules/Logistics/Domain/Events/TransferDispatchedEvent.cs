using PIMS_MS.Modules.Logistics.Domain.Entities;

namespace PIMS_MS.Modules.Logistics.Domain.Events;
public record TransferDispatchedEvent(
    Guid TransferId, 
    string TrackingCode,
    Guid OriginLocationId, 
    IReadOnlyCollection<TransferItem> Items
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}