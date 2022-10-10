using MiniESS.Core.Events;

namespace MiniESS.Core.Aggregate;

public interface IAggregateRoot : IEntity
{
   long Version { get; } 
   IReadOnlyCollection<IDomainEvent> Events { get; }
   void ClearEvents();
}