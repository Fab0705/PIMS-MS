using PIMS_MS.Modules.Inventory.Domain.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Entities;
public class Region
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;

    private Region() { }

    internal Region(Guid id, string name, string code)
    {
        if (id == Guid.Empty) 
            throw new InvalidDomainArgumentException("El ID de la región es requerido.");
        if (string.IsNullOrWhiteSpace(name)) 
            throw new InvalidDomainArgumentException("El nombre de la región es requerido.");
        if (string.IsNullOrWhiteSpace(code)) 
            throw new InvalidDomainArgumentException("El código abreviado de la región es requerido.");

        Id = id;
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
    }
}