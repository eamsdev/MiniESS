using MiniESS.Core.Events;

namespace MiniESS.Subscription.Extensions;

public static class DomainEventExtensions
{
    public static Type? GetAssociatedAggregateType(this IDomainEvent @event)
        => @event.GetType().GetGenericArguments().FirstOrDefault();
}