using MiniESS.Core.Events;

namespace MiniESS.Projection.Extensions;

public static class DomainEventExtensions
{
    public static Type? GetAssociatedAggregateType(this IDomainEvent @event)
        => @event.GetType().GetGenericArguments().FirstOrDefault();
}