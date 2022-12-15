using MiniESS.Core.Events;

namespace MiniESS.Core.Extensions;

public static class DomainEventExtensions
{
    public static Type? GetAssociatedAggregateType(this IDomainEvent @event)
        // Assuming that the one level higher than the derived type is the BaseDomainEvent<TAggregate> is risky
        // TODO: Recursive find or make IDomainEvent generic to TAggregate
        => @event.GetType().BaseType?.GenericTypeArguments.FirstOrDefault();
}