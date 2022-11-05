using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Core.Repository;
using MiniESS.Core.Tests.Models;

namespace MiniESS.Core.Tests.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection UseStubbedEventStoreClient(this IServiceCollection services)
    {
        return services
            .RemoveAll<IEventStoreClient>()
            .RemoveAll<EventStoreClient>()
            .AddTransient<IEventStoreClient, FakeEventStoreClientAdaptor>();
    }
}