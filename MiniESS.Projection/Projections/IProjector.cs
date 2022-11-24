using MiniESS.Common.Events;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;

namespace MiniESS.Projection.Projections;

// TAggregateRoot is a Generic marker to aid DI Resolution/Reflection
public interface IProjector<TAggregateRoot> : IProjector where TAggregateRoot : class, IAggregateRoot
{ }

public interface IProjector
{
    Task ProjectEventAsync(IDomainEvent @event, CancellationToken token);
}