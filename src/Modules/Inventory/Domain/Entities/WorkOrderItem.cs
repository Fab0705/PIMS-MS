using PIMS_MS.Modules.Inventory.Domain.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Entities;
public class WorkOrderItem
{
    public Guid Id { get; private set; }
    public Guid WorkOrderId { get; private set; }
    public Guid SparePartId { get; private set; }
    public int Quantity { get; private set; }

    public SparePart SparePart { get; private set; } = null!;

    private WorkOrderItem() { }

    internal WorkOrderItem(Guid id, Guid workOrderId, Guid sparePartId, int quantity)
    {
        if (id == Guid.Empty || workOrderId == Guid.Empty || sparePartId == Guid.Empty)
            throw new InvalidDomainArgumentException("Los identificadores de la orden, ítem o repuesto no pueden ser vacíos.");

        if (quantity <= 0) 
            throw new InvalidDomainArgumentException("La cantidad asignada para la orden de trabajo debe ser mayor a cero.");
            
        Id = id;
        WorkOrderId = workOrderId;
        SparePartId = sparePartId;
        Quantity = quantity;
    }

    // Permitimos que la Raíz de Agregado modifique la cantidad si se requiere reunificar ítems
    internal void AddQuantity(int additionalQuantity)
    {
        if (additionalQuantity <= 0)
            throw new InvalidDomainArgumentException("La cantidad a adicionar debe ser mayor a cero.");
            
        Quantity += additionalQuantity;
    }
}