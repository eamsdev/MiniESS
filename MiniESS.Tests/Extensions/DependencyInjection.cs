using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Infrastructure.Repository;
using MiniESS.Infrastructure.Subscriptions;
using MiniESS.Subscription.Tests.Models;

namespace MiniESS.Subscription.Tests.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection UseStubbedEventStoreSubscriberAndInMemoryDbContext(this IServiceCollection services)
    {
        return services
            .RemoveAll<IEventStoreClient>()
            .RemoveAll<EventStoreClient>()
            .AddScoped<IEventStoreClient, FakeEventStoreClientAdaptor>()
            .RemoveAll<IEventStoreSubscriber>()
            .RemoveAll<EventStoreSubscriber>()
            .AddDbContext<DummyDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ProjectionTests"))
            .AddScoped<IEventStoreSubscriber, FakeEventStoreSubscriber>();
    }
}