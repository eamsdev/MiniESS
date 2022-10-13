using EventStore.Client;

namespace MiniESS.Projection.Subscriptions;

public interface IEventStoreSubscriber
{
    Task<StreamSubscription> SubscribeToAllAsync(
        FromAll start,
        Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
        bool resolveLinkTos = false,
        Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
        SubscriptionFilterOptions? filterOptions = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);
}