using PIMS_MS.Modules.Logistics.Domain.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Entities;

public class ReplenishmentItem
{
    public Guid Id { get; private set; }
    public Guid ReplenishmentId { get; private set; }
    public Guid SparePartId { get; private set; }
    public int Quantity { get; private set; }
    private ReplenishmentItem() {}
    public ReplenishmentItem(Guid id, Guid replenishmentId, Guid sparePartId, int quantity)
    {
        if (id == Guid.Empty || replenishmentId == Guid.Empty || sparePartId == Guid.Empty)
            throw new InvalidLogisticsArgumentException("Los identificadores del ítem, solicitud o repuesto no son válidos.");

        if (quantity <= 0)
            throw new InvalidLogisticsArgumentException("La cantidad solicitada debe ser mayor a cero.");

        Id = id;
        ReplenishmentId = replenishmentId;
        SparePartId = sparePartId;
        Quantity = quantity;
    }
    internal void AddQuantity(int amount)
    {
        if (amount <= 0)
            throw new InvalidLogisticsArgumentException("La cantidad a adicionar debe ser mayor a cero.");
        Quantity += amount;
    }
}