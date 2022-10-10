namespace MiniESS.Core.Events;

public interface IDomainEvent
{
   long AggregateVersion { get; }
   Guid AggregateId { get; }
   DateTime RaisedTime { get; }
}