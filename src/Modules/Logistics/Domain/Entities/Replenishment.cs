using PIMS_MS.Modules.Logistics.Domain.Enums;
using PIMS_MS.Modules.Logistics.Domain.Events;
using PIMS_MS.Modules.Logistics.Domain.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Entities;

public class Replenishment : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid LocationId { get; private set; }
    public ReplenishmentStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    
    private readonly List<ReplenishmentItem> _items = new();
    public IReadOnlyCollection<ReplenishmentItem> Items => _items.AsReadOnly();
    
    private Replenishment() {}
    public Replenishment(Guid id, Guid locationId)
    {
        if (id == Guid.Empty || locationId == Guid.Empty)
            throw new InvalidLogisticsArgumentException("El ID de la solicitud y del almacén provincial son obligatorios.");

        Id = id;
        LocationId = locationId;
        Status = ReplenishmentStatus.Pending;
    }

    public void AddItem(Guid sparePartId, int quantity)
    {
        if (Status != ReplenishmentStatus.Pending)
            throw new InvalidReplenishmentStatusException(Status.ToString(), "Añadir Ítem");

        var existingItem = _items.FirstOrDefault(i => i.SparePartId == sparePartId);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
            return;
        }

        _items.Add(new ReplenishmentItem(Guid.NewGuid(), Id, sparePartId, quantity));
    }

    public void Approve()
    {
        if (Status != ReplenishmentStatus.Pending)
            throw new InvalidReplenishmentStatusException(Status.ToString(), "Aprobar Solicitud");

        if (!_items.Any())
            throw new EmptyLogisticsDocumentException("Solicitud de Reabastecimiento");

        Status = ReplenishmentStatus.Approved;
        
        AddDomainEvent(new ReplenishmentApprovedEvent(Id, LocationId, _items));
    }

    public void Refuse(string reason)
    {
        if (Status != ReplenishmentStatus.Pending)
            throw new InvalidReplenishmentStatusException(Status.ToString(), "Rechazar Solicitud");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidLogisticsArgumentException("Debe ingresar un motivo para rechazar la solicitud de abastecimiento.");

        Status = ReplenishmentStatus.Refused;
        RejectionReason = reason.Trim();
    }
}