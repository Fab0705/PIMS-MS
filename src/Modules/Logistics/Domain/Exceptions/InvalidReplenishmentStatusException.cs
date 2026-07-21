using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Exceptions;

public class InvalidReplenishmentStatusException : DomainException
{
    public InvalidReplenishmentStatusException(string currentStatus, string intendedAction) 
        : base($"No se puede realizar la acción '{intendedAction}' debido a que la solicitud de reabastecimiento está en estado '{currentStatus}'.")
    {
    }
}