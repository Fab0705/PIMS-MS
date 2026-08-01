using PIMS_MS.Modules.Logistics.Domain.Events;

public record ReplenishmentApprovedEvent(
    Guid ReplenishmentId, 
    Guid LocationId, 
    IReadOnlyCollection<ReplenishmentItemEventDto> Items
) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}