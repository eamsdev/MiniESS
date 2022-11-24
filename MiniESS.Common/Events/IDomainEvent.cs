namespace MiniESS.Common.Events;

public interface IDomainEvent
{
   long AggregateVersion { get; }
   Guid AggregateId { get; }
   DateTime RaisedTime { get; }
}