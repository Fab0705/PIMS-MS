using MediatR;

namespace PIMS_MS.Common.Contracts;

public record WorkOrderItemContractDto(Guid SparePartId, int Quantity);

public record WorkOrderStartWorkIntegrationEvent(
    Guid WorkOrderId, 
    string WorkOrderNumber,
    string Description,
    Guid LocationId, 
    IReadOnlyCollection<WorkOrderItemContractDto> Items
) : INotification;

public record WorkOrderCompleteWorkIntegrationEvent(
    Guid WorkOrderId, 
    string WorkOrderNumber,
    string Description,
    Guid LocationId, 
    IReadOnlyCollection<WorkOrderItemContractDto> Items
) : INotification;