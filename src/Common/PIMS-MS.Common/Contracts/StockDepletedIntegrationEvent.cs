using MediatR;

namespace PIMS_MS.Common.Contracts;

public record StockDepletedIntegrationEvent(
    Guid SparePartId,
    Guid LocationId,
    int CurrentQuantity
) : INotification;
