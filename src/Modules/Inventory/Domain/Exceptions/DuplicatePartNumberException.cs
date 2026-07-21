using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Exceptions;

public class DuplicatePartNumberException : DomainException
{
    public DuplicatePartNumberException(string partNumber) 
        : base($"El número de parte '{partNumber}' ya se encuentra registrado en el catálogo de repuestos.")
    {
    }
}