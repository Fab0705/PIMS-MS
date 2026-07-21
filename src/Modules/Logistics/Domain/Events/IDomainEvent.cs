using MediatR;

namespace PIMS_MS.Modules.Logistics.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}