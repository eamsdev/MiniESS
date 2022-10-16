using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Core.Repository;
using MiniESS.Core.Tests.Models;
using MiniESS.Projection.Subscriptions;
using MiniESS.Subscription.Tests.Models;

namespace MiniESS.Subscription.Tests.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection UseStubbedEventStoreSubscriberAndInMemoryDbContext(this IServiceCollection services)
    {
        return services
            .RemoveAll<IEventStoreClient>()
            .RemoveAll<EventStoreClient>()
            .AddSingleton<IEventStoreClient, FakeEventStoreClientAdaptor>()
            .RemoveAll<IEventStoreSubscriber>()
            .RemoveAll<EventStoreSubscriber>()
            .AddDbContext<DummyDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ProjectionTests"))
            .AddSingleton<IEventStoreSubscriber, FakeEventStoreSubscriber>();
    }
}