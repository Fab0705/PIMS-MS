using PIMS_MS.Modules.Inventory.Domain.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Entities;

public class SparePart : AggregateRoot
{
    public Guid Id { get; private set; }
    public string PartNumber { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsRework { get; private set; }
    
    private SparePart() { }

    public SparePart(Guid id, string partNumber, string description, bool isRework)
    {
        if (id == Guid.Empty)
            throw new InvalidDomainArgumentException("El ID del repuesto no puede ser un Guid vacío.");
        if (string.IsNullOrWhiteSpace(partNumber)) 
            throw new InvalidDomainArgumentException("El número de parte (PartNumber) es obligatorio.");
        if (string.IsNullOrWhiteSpace(description)) 
            throw new InvalidDomainArgumentException("La descripción del repuesto es obligatoria.");

        Id = id;
        PartNumber = partNumber.Trim().ToUpperInvariant(); // Estandarización obligatoria
        Description = description.Trim();
        IsRework = isRework;
    }

    public void UpdateDetails(string description, bool isRework)
    {
        if (string.IsNullOrWhiteSpace(description)) 
            throw new InvalidDomainArgumentException("La descripción no puede quedar vacía al actualizar el repuesto.");

        Description = description.Trim();
        IsRework = isRework;
    }
}