namespace PIMS_MS.Modules.Logistics.Domain.Enums;

public enum TransferStatus
{
    Pending = 0,
    ReadyForDispatch = 1,
    InTransit = 2,
    Exception = 3,    
    Received = 4,
}