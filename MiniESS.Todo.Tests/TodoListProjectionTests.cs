using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Tests.Models;
using MiniESS.Projection;
using MiniESS.Projection.Projections;
using MiniESS.Subscription.Tests.Models;
using MiniESS.Todo.Todo;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Tests;

public class TodoListProjectionTests
{
    private const string Title = "Title";
    
    private readonly TodoDbContext _dbContext;
    private readonly ServiceProvider _serviceProvider;
    private readonly ProjectionOrchestrator _orchestrator;
    
    public TodoListProjectionTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddProjectionService(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(TodoListAggregateRoot).Assembly };
            })
            .UseStubbedEventStoreSubscriberAndInMemoryDbContext()
            .AddProjector<TodoListAggregateRoot, TodoListProjector>()
            .BuildServiceProvider();

        _dbContext = _serviceProvider.GetRequiredService<TodoDbContext>();
        _orchestrator = _serviceProvider.GetRequiredService<ProjectionOrchestrator>();
    }

    [Fact]
    public async Task CanProjectCreateTodoEvent()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        var todoList = TodoListAggregateRoot.Create(streamId, Title);
        var @event = new TodoListEvents.TodoListCreated(todoList, Title);

        // Act
        await _orchestrator.SendToProjector(@event, CancellationToken.None);
        var readModel = await _dbContext.Set<TodoList>().FindAsync(streamId);

        // Assert
        readModel.Should().NotBeNull();
        readModel!.Id.Should().Be(streamId);
        readModel.Title.Should().Be(Title);
        readModel.TodoItems.Should().BeEmpty();
    }
}