using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Events;

public interface IDomainEvent : IEntityCorrelation
{
   long AggregateVersion { get; }
}