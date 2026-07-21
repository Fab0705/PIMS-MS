using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Exceptions;

public class InvalidTransferStatusException : DomainException
{
    public InvalidTransferStatusException(string currentStatus, string intendedAction) 
        : base($"No se puede realizar la acción '{intendedAction}' debido a que el traslado se encuentra en estado '{currentStatus}'.")
    {
    }
}