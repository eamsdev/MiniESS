using Microsoft.EntityFrameworkCore;
using MiniESS.Projection.Projections;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo;

public class TodoListProjector :
    ProjectorBase<TodoListAggregateRoot>,
    IProject<TodoListEvents.TodoListCreated>,
    IProject<TodoListEvents.TodoItemAdded>,
    IProject<TodoListEvents.TodoItemCompleted>
{
    public TodoListProjector(
        DbContext context, 
        IServiceProvider serviceProvider, 
        ILogger<ProjectorBase<TodoListAggregateRoot>> logger) : base(context, serviceProvider, logger)
    {
    }

    public async Task ProjectEvent(TodoListEvents.TodoListCreated domainEvent, CancellationToken token)
    {
        var todoList = new TodoList
        {
            Id = domainEvent.AggregateId,
            Title = domainEvent.Title,
            TodoItems = new List<TodoItem>()
        };

        await Repository<TodoList>().AddAsync(todoList, token);
        await SaveChangesAsync();
    }

    public async Task ProjectEvent(TodoListEvents.TodoItemAdded domainEvent, CancellationToken token)
    {
        var todoList = await Repository<TodoList>().FindAsync(new object?[] { domainEvent.AggregateId }, cancellationToken: token);
        if (todoList is null)
            throw new NotFoundException($"todoList with aggregate id {domainEvent.AggregateId} is not found");
            
        var todoItem = new TodoItem
        {
            Description = domainEvent.Description,
            IsComplete = false,
            ItemNumber = domainEvent.ItemNumber,
            TodoListId = todoList.Id
        };
        
        await Repository<TodoItem>().AddAsync(todoItem, token);
        await SaveChangesAsync();
    }

    public async Task ProjectEvent(TodoListEvents.TodoItemCompleted domainEvent, CancellationToken token)
    {
        var todoItem = await Repository<TodoItem>().SingleOrDefaultAsync(x =>
            x.TodoListId == domainEvent.AggregateId && x.ItemNumber == domainEvent.ItemNumber, cancellationToken: token);
        
        if (todoItem is null)
            throw new NotFoundException($"todoItem with todoList foreign key id {domainEvent.AggregateId} is not found");

        todoItem.IsComplete = true;
        await SaveChangesAsync();
    }
}