using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MiniESS.Projection.Subscriptions;

namespace MiniESS.Subscription.Tests.Models;

public class FakeEventStoreSubscriber : IEventStoreSubscriber
{
    public FromAll? SubscriptionStartingPoint { get; set; }

    public async Task<StreamSubscription> SubscribeToAllAsync(FromAll start, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared, bool resolveLinkTos = false,
        Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null, SubscriptionFilterOptions? filterOptions = null,
        UserCredentials? userCredentials = null, CancellationToken cancellationToken = default)
    {
        SubscriptionStartingPoint = start; 
        return await Task.FromResult<StreamSubscription>(null);
    }
}