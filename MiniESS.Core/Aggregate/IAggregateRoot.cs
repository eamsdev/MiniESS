using MiniESS.Core.Events;

namespace MiniESS.Core.Aggregate;

public interface IAggregateRoot<out TKey> : IEntity<TKey>
{
   long Version { get; } 
   IReadOnlyCollection<IDomainEvent<TKey>> Events { get; }
   void ClearEvents();
}