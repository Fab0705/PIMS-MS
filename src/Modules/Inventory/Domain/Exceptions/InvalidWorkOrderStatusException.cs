using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Exceptions;

public class InvalidWorkOrderStatusException : DomainException
{
    public InvalidWorkOrderStatusException(string currentStatus, string intendedAction) 
        : base($"No se puede realizar la acción '{intendedAction}' debido a que la orden de trabajo se encuentra en estado '{currentStatus}'.")
    {
    }
}