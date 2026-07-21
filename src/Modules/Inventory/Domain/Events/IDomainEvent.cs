using MediatR;

namespace PIMS_MS.Modules.Inventory.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}