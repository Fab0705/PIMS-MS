using PIMS_MS.Modules.Logistics.Domain.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Entities;

public class TransferItem
{
    public Guid Id { get; private set; }
    public Guid TransferId { get; private set; }
    public Guid SparePartId { get; private set; }
    public int Quantity { get; private set; }

    private TransferItem() {}
    public TransferItem(Guid id, Guid transferId, Guid sparePartId, int quantity)
    {
        if (id == Guid.Empty || transferId == Guid.Empty || sparePartId == Guid.Empty)
            throw new InvalidLogisticsArgumentException("Los identificadores del ítem, traslado o repuesto son inválidos.");

        if (quantity <= 0)
            throw new InvalidLogisticsArgumentException("La cantidad a trasladar debe ser mayor a cero.");

        Id = id;
        TransferId = transferId;
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