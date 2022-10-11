using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;

namespace MiniESS.Subscription.Projections;

// TAggregateRoot is a Generic marker to aid DI Resolution
public interface IProjector<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
    Task ProjectEventsAsync(Guid aggregateId, IReadOnlyCollection<IDomainEvent> events);
}