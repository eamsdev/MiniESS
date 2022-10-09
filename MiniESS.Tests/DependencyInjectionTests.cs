using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core;
using MiniESS.Core.Repository;
using MiniESS.Core.Serialization;
using MiniESS.Tests.Extensions;
using MiniESS.Tests.Models;
using Xunit;

namespace MiniESS.Tests;

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
            .AddEventSourcingRepository<Dummy, Guid>()
            .UseStubbedEventStoreClient()
            .BuildServiceProvider();
    }

    [Fact]
    public void CanResolveRepository()
    {
        // Act
        var repository = _serviceProvider.GetService(typeof(IAggregateRepository<Dummy, Guid>));
        
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