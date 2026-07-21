using PIMS_MS.Modules.Inventory.Domain.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Entities;

public class Location
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid RegionId { get; private set; }

    public Region Region { get; private set; } = null!;

    private Location() { }

    public Location(Guid id, string name, Guid regionId)
    {
        if (id == Guid.Empty || regionId == Guid.Empty) 
            throw new InvalidDomainArgumentException("Los identificadores de ubicación y región son obligatorios.");
        if (string.IsNullOrWhiteSpace(name)) 
            throw new InvalidDomainArgumentException("El nombre del almacén o ubicación es obligatorio.");

        Id = id;
        Name = name.Trim();
        RegionId = regionId;
    }
}