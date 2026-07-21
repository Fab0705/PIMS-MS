using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public Guid SparePartId { get; }
    public string PartNumber { get; }

    public InsufficientStockException(Guid sparePartId, string partNumber) 
        : base($"No hay suficiente stock en el almacén provincial para cubrir la demanda del repuesto especificado.")
    {
        SparePartId = sparePartId;
        PartNumber = partNumber;
    }

    public InsufficientStockException(Guid sparePartId, string partNumber, int currentQuantity, int requestedQuantity) 
        : base($"Stock insuficiente para el repuesto {partNumber} ({sparePartId}). Stock actual: {currentQuantity}, Cantidad solicitada: {requestedQuantity}.")
    {
        SparePartId = sparePartId;
        PartNumber = partNumber;
    }
}