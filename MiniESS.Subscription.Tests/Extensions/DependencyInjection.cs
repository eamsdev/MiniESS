using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Subscription.Subscriptions;
using MiniESS.Subscription.Tests.Models;

namespace MiniESS.Subscription.Tests.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection UseStubbedEventStoreSubscriber(this IServiceCollection services)
    {
        return services
            .RemoveAll<IEventStoreSubscriber>()
            .RemoveAll<EventStoreSubscriber>()
            .AddSingleton<IEventStoreSubscriber, FakeEventStoreSubscriber>();
    }
}