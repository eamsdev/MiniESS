﻿using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniESS.Infrastructure.Repository;
using MiniESS.Infrastructure.Subscriptions;
using MiniESS.Subscription.Tests.Models;
using MiniESS.Todo.Todo.ReadModels;

namespace MiniESS.Todo.Tests;

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
            .AddDbContext<TodoDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("ProjectionTests"))
            .AddSingleton<IEventStoreSubscriber, FakeEventStoreSubscriber>();
    }
}