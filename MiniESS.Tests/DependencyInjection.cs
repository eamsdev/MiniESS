using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Repository;
using MiniESS.Tests.Models;

namespace MiniESS.Tests;

public static class DependencyInjection
{
    public static IServiceCollection UseStubbedEventStoreClient(this IServiceCollection services)
    {
        return services
            .RemoveAll<IEventStoreClientAdaptor>()
            .RemoveAll<EventStoreClient>()
            .AddSingleton<IEventStoreClientAdaptor, FakeEventStoreClientAdaptor>();
    }
}