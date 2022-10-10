using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Repository;
using MiniESS.Core.Tests.Extensions;
using MiniESS.Core.Tests.Models;
using Xunit;

namespace MiniESS.Core.Tests;

public class AggregateRepositoryTests
{
    private readonly IAggregateRepository<Dummy> _dummyAggregateRepository;

    public AggregateRepositoryTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddEventSourcing(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .AddEventSourcingRepository<Dummy>()
            .UseStubbedEventStoreClient()
            .BuildServiceProvider();

        _dummyAggregateRepository = serviceProvider.GetService(typeof(IAggregateRepository<Dummy>)) as IAggregateRepository<Dummy> 
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