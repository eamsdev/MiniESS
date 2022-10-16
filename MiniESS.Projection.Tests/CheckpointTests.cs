using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using EventStore.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Repository;
using MiniESS.Core.Tests.Models;
using MiniESS.Projection;
using MiniESS.Projection.Subscriptions;
using MiniESS.Subscription.Tests.Extensions;
using MiniESS.Subscription.Tests.Models;
using Xunit;

namespace MiniESS.Subscription.Tests;

public class CheckpointTests
{
    private readonly EventStoreSubscribeToAll _subscribeToAll;
    private readonly FakeEventStoreSubscriber _fakeSubscriber;
    private readonly FakeEventStoreClientAdaptor _fakeEventStoreClient;
    private readonly SubscriptionCheckpointRepository _checkpointRepository;

    public CheckpointTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddProjectionService(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .UseStubbedEventStoreSubscriberAndInMemoryDbContext()
            .BuildServiceProvider();

        _subscribeToAll = serviceProvider.GetRequiredService<EventStoreSubscribeToAll>();
        _checkpointRepository = serviceProvider.GetRequiredService<SubscriptionCheckpointRepository>();
        _fakeSubscriber = (serviceProvider.GetRequiredService<IEventStoreSubscriber>() as FakeEventStoreSubscriber)!;
        _fakeEventStoreClient = (serviceProvider.GetRequiredService<IEventStoreClient>() as FakeEventStoreClientAdaptor)!;
    }

    [Fact]
    public async void SubscribeFromStartWhenNoExistingCheckpoints()
    {
        // Arrange
        
        // Act 
        await _subscribeToAll.SubscribeToAll(CancellationToken.None);
        
        // Assert
        _fakeSubscriber.SubscriptionStartingPoint.Should().Be(FromAll.Start);
    }
    
    [Fact]
    public async void SubscribeFromAfterCheckpointPointWhenCheckpointExists()
    {
        // Arrange
        const ulong checkpoint = 999;
        var expectedStartingPosition = new Position(checkpoint, checkpoint);
        await _checkpointRepository.Store("ToAll", checkpoint, CancellationToken.None);
        
        // Act 
        await _subscribeToAll.SubscribeToAll(CancellationToken.None);
        
        // Assert
        _fakeSubscriber.SubscriptionStartingPoint!.Value.ToUInt64().commitPosition.Should().Be(expectedStartingPosition.CommitPosition);
        _fakeSubscriber.SubscriptionStartingPoint!.Value.ToUInt64().preparePosition.Should().Be(expectedStartingPosition.PreparePosition);
    }
    
    [Fact]
    public async void StoringNewCheckpointWillSetStreamMetaDataAndAppendEvent()
    {
        // Arrange
        const ulong checkpoint = 999;
        var expectedStartingPosition = new Position(checkpoint, checkpoint);
        _fakeEventStoreClient.ConfigureWrongVersionExceptionOnAppend(
            "checkpoint_ToAll", 
            StreamRevision.FromStreamPosition(StreamPosition.FromInt64((long)checkpoint)), 
            StreamRevision.None);
        
        // Act 
        await _checkpointRepository.Store("ToAll", checkpoint, CancellationToken.None);
        await _subscribeToAll.SubscribeToAll(CancellationToken.None);
        
        // Assert
        _fakeEventStoreClient.StreamMetaDataStreamName.Should().Be("checkpoint_ToAll");
        _fakeEventStoreClient.StreamMetaData!.Value.MaxCount.Should().Be(1);
        _fakeEventStoreClient.StreamMetaDataStreamState!.Value.Should().Be(StreamState.NoStream);
        _fakeSubscriber.SubscriptionStartingPoint!.Value.ToUInt64().commitPosition.Should().Be(expectedStartingPosition.CommitPosition);
        _fakeSubscriber.SubscriptionStartingPoint!.Value.ToUInt64().preparePosition.Should().Be(expectedStartingPosition.PreparePosition);
    }
}