namespace PIMS_MS.Modules.Logistics.Domain.Events;

public record ReplenishmentItemEventDto(Guid SparePartId, int Quantity);