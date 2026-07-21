using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Inventory.Domain.Exceptions;

public class InvalidDomainArgumentException : DomainException
{
    public InvalidDomainArgumentException(string message) : base(message)
    {
    }
}