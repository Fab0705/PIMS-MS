using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Exceptions;

public class EmptyLogisticsDocumentException : DomainException
{
    public EmptyLogisticsDocumentException(string documentType) 
        : base($"El documento logístico '{documentType}' no puede procesarse porque no contiene ítems asignados.")
    {
    }
}