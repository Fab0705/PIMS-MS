using PIMS_MS.Modules.Inventory.Domain.Enums;
using PIMS_MS.Modules.Inventory.Domain.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Entities;

public class WorkOrder : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid LocationId { get; private set; }
    public string WorkOrderNumber { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public WorkOrderStatus Status { get; private set; }

    // Listado encapsulado (DDD)
    private readonly List<WorkOrderItem> _items = new();
    public IReadOnlyCollection<WorkOrderItem> Items => _items.AsReadOnly();

    public Location Location { get; private set; } = null!;

    private WorkOrder() { }

    public WorkOrder(Guid id, Guid locationId, string workOrderNumber, string description)
    {
        if (id == Guid.Empty || locationId == Guid.Empty)
            throw new InvalidDomainArgumentException("Los IDs de la orden y del almacén son obligatorios.");
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            throw new InvalidDomainArgumentException("El número de orden de trabajo es obligatorio.");
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidDomainArgumentException("La descripción de la orden de trabajo es obligatoria.");

        Id = id;
        LocationId = locationId;
        WorkOrderNumber = workOrderNumber.Trim().ToUpperInvariant();
        Description = description.Trim();
        Status = WorkOrderStatus.Pending;
    }

    public void AddItem(Guid sparePartId, int quantity)
    {
        if (Status != WorkOrderStatus.Pending) 
            throw new InvalidWorkOrderStatusException(Status.ToString(), "Añadir Ítem");
        
        // Regla de Negocio: Evitar repuestos duplicados en la misma orden
        var existingItem = _items.FirstOrDefault(i => i.SparePartId == sparePartId);
        if (existingItem != null)
        {
            // Opcional: En vez de lanzar DuplicateWorkOrderItemException, podemos sumar la cantidad
            existingItem.AddQuantity(quantity);
            return;
        }

        _items.Add(new WorkOrderItem(Guid.NewGuid(), Id, sparePartId, quantity));
    }

    public void StartWork()
    {
        if (Status != WorkOrderStatus.Pending)
            throw new InvalidWorkOrderStatusException(Status.ToString(), "Iniciar Trabajo");

        if (!_items.Any())
            throw new EmptyWorkOrderException();

        Status = WorkOrderStatus.InProgress;
    }

    public void CompleteWork()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidWorkOrderStatusException(Status.ToString(), "Completar Trabajo");

        Status = WorkOrderStatus.Completed;
    }
}