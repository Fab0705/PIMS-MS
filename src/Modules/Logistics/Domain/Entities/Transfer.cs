using PIMS_MS.Modules.Logistics.Domain.Enums;
using PIMS_MS.Modules.Logistics.Domain.Events;
using PIMS_MS.Modules.Logistics.Domain.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Entities;

public class Transfer : AggregateRoot
{
    public Guid Id { get; private set; }
    public string TrackingCode { get; private set; } = null!;
    public Guid OriginLocationId { get; private set; }
    public Guid DestinationLocationId { get; private set; }
    public TransferStatus Status { get; private set; }
    public string? ExceptionNotes { get; private set; }
    
    private readonly List<TransferItem> _items = new();
    public IReadOnlyCollection<TransferItem> Items => _items.AsReadOnly();

    private Transfer() {}
    public Transfer(Guid id, string trackingCode, Guid originLocationId, Guid destinationLocationId)
    {
        if (id == Guid.Empty)
            throw new InvalidLogisticsArgumentException("El ID del traslado no es válido.");

        if (string.IsNullOrWhiteSpace(trackingCode))
            throw new InvalidLogisticsArgumentException("El código de rastreo (TrackingCode) es obligatorio.");

        if (originLocationId == Guid.Empty || destinationLocationId == Guid.Empty)
            throw new InvalidLogisticsArgumentException("Los almacenes de origen y destino son requeridos.");

        if (originLocationId == destinationLocationId)
            throw new SameOriginAndDestinationException(originLocationId);

        Id = id;
        TrackingCode = trackingCode.Trim().ToUpperInvariant();
        OriginLocationId = originLocationId;
        DestinationLocationId = destinationLocationId;
        Status = TransferStatus.Pending;
    }
    public void AddItem(Guid sparePartId, int quantity)
    {
        if(Status != TransferStatus.Pending)
            throw new InvalidTransferStatusException(Status.ToString(), "Añadir Ítem");
        
        var existingItem = _items.FirstOrDefault(i => i.SparePartId == sparePartId);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
            return;
        }

        _items.Add(new TransferItem(Guid.NewGuid(), Id, sparePartId, quantity));
    }
    public void MarkAsReadyForDispatch()
    {
        if (Status != TransferStatus.Pending)
            throw new InvalidTransferStatusException(Status.ToString(), "Preparar Despacho");

        if (!_items.Any())
            throw new EmptyLogisticsDocumentException("Guía de Traslado interprovincial");

        Status = TransferStatus.ReadyForDispatch;
    }
    public void Dispatch()
    {
        if (Status != TransferStatus.ReadyForDispatch && Status != TransferStatus.Pending)
            throw new InvalidTransferStatusException(Status.ToString(), "Despachar Traslado");

        if (!_items.Any())
            throw new EmptyLogisticsDocumentException("Guía de Traslado interprovincial");

        Status = TransferStatus.InTransit;

        AddDomainEvent(new TransferDispatchedEvent(Id, TrackingCode, OriginLocationId, _items));
    }
    public void Receive()
    {
        if (Status != TransferStatus.InTransit)
            throw new InvalidTransferStatusException(Status.ToString(), "Recibir Traslado");

        Status = TransferStatus.Received;

        AddDomainEvent(new TransferReceivedEvent(Id, TrackingCode, DestinationLocationId, _items));
    }
    public void RegisterException(string notes)
    {
        if (Status == TransferStatus.Received)
            throw new InvalidTransferStatusException(Status.ToString(), "Registrar Excepción");

        if (string.IsNullOrWhiteSpace(notes))
            throw new InvalidLogisticsArgumentException("Debe especificar el detalle de la incidencia o problema en el traslado.");

        Status = TransferStatus.Exception;
        ExceptionNotes = notes.Trim();
    }
}