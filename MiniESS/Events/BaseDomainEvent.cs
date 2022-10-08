using MiniESS.Aggregate;

namespace MiniESS.Events;

public abstract record BaseDomainEvent<TAggregateRoot, TKey> : IDomainEvent<TKey> 
    where TAggregateRoot : IAggregateRoot<TKey>
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

    public long AggregateVersion { get; private set; }
    public TKey AggregateId { get; private set; }
    public DateTime RaisedTime { get; private set; }
}