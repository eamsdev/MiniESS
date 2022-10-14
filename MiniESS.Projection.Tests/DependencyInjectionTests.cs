using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Tests.Models;
using MiniESS.Projection;
using MiniESS.Projection.Projections;
using MiniESS.Projection.Subscriptions;
using MiniESS.Subscription.Tests.Extensions;
using Xunit;

namespace MiniESS.Subscription.Tests;

public class DependencyInjectionTests
{
    private readonly ServiceProvider _serviceProvider;

    public DependencyInjectionTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddProjectionService(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .UseStubbedEventStoreSubscriberAndInMemeoryDbContext()
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
    
    [Fact]
    public void CanResolveOrchestrator()
    {
        // Act
        var orchestrator = _serviceProvider.GetService(typeof(ProjectionOrchestrator));
        // Assert
        orchestrator.Should().NotBeNull();
    }
}