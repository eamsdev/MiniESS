using EventStore.Client;
using MiniESS.Core.Repository;

namespace MiniESS.Subscription.Subscriptions;

public class EventStoreSubscriber : IEventStoreSubscriber
{
    private readonly EventStoreClient _client;

    public EventStoreSubscriber(EventStoreClient client) 
    {
        _client = client;
    }


    public async Task<StreamSubscription> SubscribeToAllAsync(
        FromAll start, 
        Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared, 
        bool resolveLinkTos = false,
        Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null, 
        SubscriptionFilterOptions? filterOptions = null,
        UserCredentials? userCredentials = null, 
        CancellationToken cancellationToken = default)
    {
        return await _client.SubscribeToAllAsync(start, 
            eventAppeared, 
            resolveLinkTos, 
            subscriptionDropped, 
            filterOptions, 
            userCredentials, 
            cancellationToken: cancellationToken);
    }
}