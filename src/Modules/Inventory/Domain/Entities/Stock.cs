using PIMS_MS.Modules.Inventory.Domain.Events;
using PIMS_MS.Modules.Inventory.Domain.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Entities;

public class Stock : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid SparePartId { get; private set; }
    public Guid LocationId { get; private set; }
    public int Quantity { get; private set; }

    public SparePart SparePart { get; private set; } = null!;
    public Location Location { get; private set; } = null!;

    private Stock() { }

    public Stock(Guid id, Guid sparePartId, Guid locationId, int initialQuantity)
    {
        if (id == Guid.Empty || sparePartId == Guid.Empty || locationId == Guid.Empty)
            throw new InvalidDomainArgumentException("Los identificadores de stock, repuesto y almacén son obligatorios.");
        if (initialQuantity < 0) 
            throw new InvalidDomainArgumentException("La cantidad inicial de stock no puede ser negativa.");

        Id = id;
        SparePartId = sparePartId;
        LocationId = locationId;
        Quantity = initialQuantity;
    }

    // Métodos DDD para mutar el estado de manera segura
    public void Increment(int amount)
    {
        if (amount <= 0) 
            throw new InvalidDomainArgumentException("La cantidad a incrementar debe ser mayor a cero.");

        Quantity += amount;
    }

    public void Decrement(int amount)
    {
        if (amount <= 0) 
            throw new InvalidDomainArgumentException("La cantidad a descontar debe ser mayor a cero.");

        if (Quantity - amount < 0) 
            throw new InsufficientStockException(SparePartId, SparePart.PartNumber, Quantity, amount);

        Quantity -= amount;

        // Disparar Evento de Dominio si cruza el umbral crítico (ej. menos de 5 unidades)
        if (Quantity <= 5)
        {
            AddDomainEvent(new StockDepletedEvent(SparePartId, LocationId, Quantity));
        }
    }

    public void AdjustToPhysicalCount(int newPhysicalCount, string reason)
    {
        if (newPhysicalCount < 0) 
            throw new InvalidDomainArgumentException("El conteo físico no puede registrar stock negativo.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidDomainArgumentException("Debe proveer una razón para el ajuste manual de inventario.");

        Quantity = newPhysicalCount;
    }
}