namespace MiniESS.Events;

public interface IDomainEvent<out TKey> 
{
   long AggregateVersion { get; }
   TKey AggregateId { get; }
   DateTime RaisedTime { get; }
}