using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Repository;
using MiniESS.Tests.Extensions;
using MiniESS.Tests.Models;
using Xunit;

namespace MiniESS.Tests;

public class AggregateRepositoryTests
{
    private readonly IAggregateRepository<Dummy, Guid> _dummyAggregateRepository;

    public AggregateRepositoryTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddEventSourcing(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .AddEventSourcingRepository<Dummy, Guid>()
            .UseStubbedEventStoreClient()
            .BuildServiceProvider();

        _dummyAggregateRepository = serviceProvider.GetService(typeof(IAggregateRepository<Dummy, Guid>)) as IAggregateRepository<Dummy, Guid> 
                                    ?? throw new InvalidOperationException();
    }

    [Fact]
    public async void CanPersistNewAggregate()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        var dummy = Dummy.Create(streamId);
        
        // Act
        await _dummyAggregateRepository.PersistAsync(dummy, CancellationToken.None);
        
        // Assert
        var hydratedDummy = await _dummyAggregateRepository.LoadAsync(streamId, CancellationToken.None);
        hydratedDummy.Should().NotBeNull();
        hydratedDummy!.Count.Should().Be(0);
        hydratedDummy.Flag.Should().BeFalse();
    }

    [Fact]
    public async void CanAddEventsToAggregate()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        await _dummyAggregateRepository.PersistAsync(Dummy.Create(streamId), CancellationToken.None);
        var dummy = await _dummyAggregateRepository.LoadAsync(streamId, CancellationToken.None);
       
        // Act 
        dummy!.IncrementCount();
        dummy.IncrementCount();
        dummy.SetFlag(true);
        await _dummyAggregateRepository.PersistAsync(dummy, CancellationToken.None);
        
        // Assert
        var hydratedDummy = await _dummyAggregateRepository.LoadAsync(streamId, CancellationToken.None);
        hydratedDummy.Should().NotBeNull();
        hydratedDummy!.Count.Should().Be(2);
        hydratedDummy.Flag.Should().BeTrue();
    }
    
}