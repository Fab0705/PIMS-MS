using MediatR;

namespace PIMS_MS.Common.Contracts;

public record TransferItemContractDto(Guid SparePartId, int Quantity);

public record TransferDispatchedIntegrationEvent(
    Guid TransferId, 
    string TrackingCode,
    Guid OriginLocationId, 
    IReadOnlyCollection<TransferItemContractDto> Items
) : INotification;

public record TransferReceivedIntegrationEvent(
    Guid TransferId, 
    string TrackingCode,
    Guid DestinationLocationId, 
    IReadOnlyCollection<TransferItemContractDto> Items
) : INotification;