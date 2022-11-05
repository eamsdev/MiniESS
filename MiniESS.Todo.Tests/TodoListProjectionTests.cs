using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Projection;
using MiniESS.Projection.Projections;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Tests;

public class TodoListProjectionTests
{
    private const string Title = "Title";
    
    private readonly TodoDbContext _dbContext;
    private readonly ProjectionOrchestrator _orchestrator;
    
    public TodoListProjectionTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddProjectionService(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(TodoListAggregateRoot).Assembly };
            })
            .UseStubbedEventStoreSubscriberAndInMemoryDbContext()
            .AddProjector<TodoListAggregateRoot, TodoListProjector>()
            .BuildServiceProvider();

        _dbContext = serviceProvider.GetRequiredService<TodoDbContext>();
        _orchestrator = serviceProvider.GetRequiredService<ProjectionOrchestrator>();
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
        var readModel = await _dbContext.Set<TodoList>()
            .Include(x => x.TodoItems)
            .SingleOrDefaultAsync(x => x.Id == streamId);

        // Assert
        readModel.Should().NotBeNull();
        readModel!.Id.Should().Be(streamId);
        readModel.Title.Should().Be(Title);
        readModel.TodoItems.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CanProjectAddTodoItemsEvent()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        var todoList = TodoListAggregateRoot.Create(streamId, Title);
        var created = new TodoListEvents.TodoListCreated(todoList, Title);
        var firstItem = new TodoListEvents.TodoItemAdded(todoList, 0, "Foobar0");
        var secondItem = new TodoListEvents.TodoItemAdded(todoList, 1, "Foobar1");

        // Act
        await _orchestrator.SendToProjector(created, CancellationToken.None);
        await _orchestrator.SendToProjector(firstItem, CancellationToken.None);
        await _orchestrator.SendToProjector(secondItem, CancellationToken.None);
        var readModel = await _dbContext
            .Set<TodoList>()
            .Include(x => x.TodoItems)
            .SingleOrDefaultAsync(x => x.Id == streamId);

        // Assert
        readModel.Should().NotBeNull();
        readModel!.TodoItems.Count.Should().Be(2);
        readModel.TodoItems.SingleOrDefault(x => x.ItemNumber == 0 && x.Description == "Foobar0").Should().NotBeNull();
        readModel.TodoItems.SingleOrDefault(x => x.ItemNumber == 1 && x.Description == "Foobar1").Should().NotBeNull();
    }
    
    [Fact]
    public async Task CanProjectTodoItemCompletedEvent()
    {
        // Arrange
        var streamId = Guid.NewGuid();
        var todoList = TodoListAggregateRoot.Create(streamId, Title);
        var created = new TodoListEvents.TodoListCreated(todoList, Title);
        var firstItem = new TodoListEvents.TodoItemAdded(todoList, 0, "Foobar0");
        var secondItem = new TodoListEvents.TodoItemAdded(todoList, 1, "Foobar1");
        var completeSecondItem = new TodoListEvents.TodoItemCompleted(todoList, 1);

        // Act
        await _orchestrator.SendToProjector(created, CancellationToken.None);
        await _orchestrator.SendToProjector(firstItem, CancellationToken.None);
        await _orchestrator.SendToProjector(secondItem, CancellationToken.None);
        await _orchestrator.SendToProjector(completeSecondItem, CancellationToken.None);
        var readModel = await _dbContext
            .Set<TodoList>()
            .Include(x => x.TodoItems)
            .SingleOrDefaultAsync(x => x.Id == streamId);

        // Assert
        readModel.Should().NotBeNull();
        readModel!.TodoItems.Count.Should().Be(2);
        readModel.TodoItems.SingleOrDefault(x => x.IsComplete).Should().NotBeNull();
        readModel.TodoItems.SingleOrDefault(x => x.ItemNumber == 1 && x.Description == "Foobar1" && x.IsComplete).Should().NotBeNull();
    }
}