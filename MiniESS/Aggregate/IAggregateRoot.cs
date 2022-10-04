namespace MiniESS.Aggregate;

public interface IAggregateRoot<out TKey> : IEntity<TKey>
{
   long Version { get; } 
   IReadOnlyCollection<IDomainEvent<TKey>> Events { get; }
   void ClearEvents();
}

public interface IDomainEvent<out TKey>
{
    long AggregateVersion { get; }
    TKey AggregateId { get; }
    DateTime When { get; }
}