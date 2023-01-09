using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Infrastructure;
using MiniESS.Subscription.Tests.Extensions;
using MiniESS.Subscription.Tests.Models;
using Xunit;

namespace MiniESS.Subscription.Tests;

public class AggregateRepositoryTests
{
    private readonly IAggregateRepository<Dummy> _dummyAggregateRepository;
    private readonly CommandProcessor _commandProcessor;

    public AggregateRepositoryTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddEventSourcing(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .AddEventSourcingRepository<Dummy>()
            .UseStubbedEventStoreSubscriberAndInMemoryDbContext()
            .BuildServiceProvider();

        _dummyAggregateRepository = serviceProvider.GetService(typeof(IAggregateRepository<Dummy>)) as IAggregateRepository<Dummy> 
                                    ?? throw new InvalidOperationException();
        
        _commandProcessor = serviceProvider.GetService(typeof(CommandProcessor)) as CommandProcessor 
                                    ?? throw new InvalidOperationException();
    }

    [Fact]
    public async void CanCreateNewAggregateViaCommandProcessor()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        
        // Act
        await _commandProcessor.ProcessAndCommit(
            new CreateDummy(streamId), CancellationToken.None);
        
        // Assert
        var hydratedDummy = await _dummyAggregateRepository.LoadAsync(streamId, CancellationToken.None);
        hydratedDummy.Should().NotBeNull();
        hydratedDummy!.Count.Should().Be(0);
        hydratedDummy.Flag.Should().BeFalse();
    }
    
    [Fact]
    public async void CanAddSubsequentCommandsToExistingAggregate()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        await _commandProcessor.ProcessAndCommit(
            new CreateDummy(streamId), 
            CancellationToken.None);
        
        // Act
        await _commandProcessor.ProcessAndCommit(
            new IncrementDummyCount(streamId), CancellationToken.None);
        await _commandProcessor.ProcessAndCommit(
            new IncrementDummyCount(streamId), CancellationToken.None);
        await _commandProcessor.ProcessAndCommit(
            new SetDummyFlag(streamId, true), CancellationToken.None);
        
        // Assert
        var hydratedDummy = await _dummyAggregateRepository.LoadAsync(streamId, CancellationToken.None);
        hydratedDummy.Should().NotBeNull();
        hydratedDummy!.Count.Should().Be(2);
        hydratedDummy.Flag.Should().BeTrue();
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