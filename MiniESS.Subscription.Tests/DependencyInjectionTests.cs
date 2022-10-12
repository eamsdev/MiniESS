using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Events;
using MiniESS.Core.Tests.Models;
using MiniESS.Subscription.Subscriptions;
using MiniESS.Subscription.Tests.Extensions;
using Xunit;

namespace MiniESS.Subscription.Tests;

public class DependencyInjectionTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly List<IDomainEvent> _receivedDomainEvents;

    public DependencyInjectionTests()
    {
        _receivedDomainEvents = new List<IDomainEvent>();
        _serviceProvider = new ServiceCollection()
            .AddSubscriptionAction(option =>
            {
                option.ConnectionString = "dont care lol";
                option.HandleAction = (@event, _) => _receivedDomainEvents.Add(@event);
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .UseStubbedEventStoreSubscriber()
            .BuildServiceProvider();
    }
    
    [Fact]
    public void CanResolveSubscriber()
    {
        // Act
        var subscriber = _serviceProvider.GetService(typeof(IEventStoreSubscriber));
        // Assert
        subscriber.Should().NotBeNull();
    }
}