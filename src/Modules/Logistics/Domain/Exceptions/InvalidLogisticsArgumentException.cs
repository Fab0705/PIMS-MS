using PIMS_MS.Common.Exceptions;

namespace PIMS_MS.Modules.Logistics.Domain.Exceptions;

public class InvalidLogisticsArgumentException : DomainException
{
    public InvalidLogisticsArgumentException(string message) : base(message)
    {
    }
}