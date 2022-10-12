using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;

namespace MiniESS.Subscription.Projections;

// TAggregateRoot is a Generic marker to aid DI Resolution
public interface IProjector<TAggregateRoot> : IProjector where TAggregateRoot : class, IAggregateRoot
{ }

public interface IProjector
{
    Task ProjectEventAsync(IDomainEvent @event);
}