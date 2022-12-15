namespace MiniESS.Core.Events;

public interface IDomainEvent
{
   long AggregateVersion { get; }
}