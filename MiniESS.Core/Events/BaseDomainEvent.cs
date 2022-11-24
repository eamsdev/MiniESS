using MiniESS.Common.Events;
using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Events;

public abstract record BaseDomainEvent<TAggregateRoot> : IDomainEvent
    where TAggregateRoot : IAggregateRoot
{
    protected BaseDomainEvent() { }

    protected BaseDomainEvent(TAggregateRoot aggregateRoot)
    {
        if (aggregateRoot is null)
            throw new ArgumentNullException(nameof(aggregateRoot));

        AggregateVersion = aggregateRoot.Version;
        AggregateId = aggregateRoot.StreamId;
        RaisedTime = DateTime.UtcNow;
    }

    public long AggregateVersion { get; init; }
    public Guid AggregateId { get; init; }
    public DateTime RaisedTime { get; init; }
}