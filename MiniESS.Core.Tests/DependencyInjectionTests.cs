using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Common.Serialization;
using MiniESS.Core.Repository;
using MiniESS.Core.Tests.Extensions;
using MiniESS.Core.Tests.Models;
using Xunit;

namespace MiniESS.Core.Tests;

public class DependencyInjectionTests
{
    private readonly ServiceProvider _serviceProvider;

    public DependencyInjectionTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddEventSourcing(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .AddEventSourcingRepository<Dummy>()
            .UseStubbedEventStoreClient()
            .BuildServiceProvider();
    }

    [Fact]
    public void CanResolveRepository()
    {
        // Act
        var repository = _serviceProvider.GetService(typeof(IAggregateRepository<Dummy>));
        
        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void CanResolveSerializer()
    {
        // Act
        var serializer = _serviceProvider.GetService(typeof(EventSerializer));
        
        // Assert
        serializer.Should().NotBeNull();
    }

    [Fact]
    public void CanResolveEventStoreClientAdaptor()
    {
        // Act
        var client = _serviceProvider.GetService(typeof(IEventStoreClient));
        
        // Assert
        (client as FakeEventStoreClientAdaptor).Should().NotBeNull();
    }
}