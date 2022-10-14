using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Tests.Models;
using MiniESS.Projection;
using MiniESS.Projection.Projections;
using MiniESS.Subscription.Tests.Extensions;
using MiniESS.Subscription.Tests.Models;
using Xunit;

namespace MiniESS.Subscription.Tests;

public class ProjectionTests
{
    private readonly DummyDbContext _dbContext;
    private readonly ServiceProvider _serviceProvider;
    private readonly ProjectionOrchestrator _orchestrator;

    public ProjectionTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddProjectionService(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .UseStubbedEventStoreSubscriberAndInMemeoryDbContext()
            .AddProjector<Dummy, DummyProjector>()
            .BuildServiceProvider();

        _dbContext = _serviceProvider.GetRequiredService<DummyDbContext>();
        _orchestrator = _serviceProvider.GetRequiredService<ProjectionOrchestrator>();
    }

    [Fact]
    public async Task CanProjectDummyCreatedEvent()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        var dummy = Dummy.Create(streamId);
        var @event = new DummyEvents.DummyCreated(dummy);

        // Act
        await _orchestrator.SendToProjector(@event, CancellationToken.None);
        var readModel = await _dbContext.Set<DummyReadModel>().FindAsync(streamId);

        // Assert
        readModel.Should().NotBeNull();
        readModel!.Count.Should().Be(0);
    }
}