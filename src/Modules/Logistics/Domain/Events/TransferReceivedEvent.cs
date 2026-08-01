namespace PIMS_MS.Modules.Logistics.Domain.Events;

public record TransferReceivedEvent(
    Guid TransferId, 
    string TrackingCode,
    Guid DestinationLocationId, 
    IReadOnlyCollection<TransferItemEventDto> Items
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}