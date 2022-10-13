using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Projection.Subscriptions;
using MiniESS.Subscription.Tests.Models;

namespace MiniESS.Subscription.Tests.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection UseStubbedEventStoreSubscriberAndInMemeoryDbContext(this IServiceCollection services)
    {
        return services
            .RemoveAll<IEventStoreSubscriber>()
            .RemoveAll<EventStoreSubscriber>()
            .AddDbContext<DummyDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ProjectionTests"))
            .AddSingleton<IEventStoreSubscriber, FakeEventStoreSubscriber>();
    }
}