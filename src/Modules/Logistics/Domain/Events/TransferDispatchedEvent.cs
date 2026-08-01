namespace PIMS_MS.Modules.Logistics.Domain.Events;
public record TransferDispatchedEvent(
    Guid TransferId, 
    string TrackingCode,
    Guid OriginLocationId, 
    IReadOnlyCollection<TransferItemEventDto> Items
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}