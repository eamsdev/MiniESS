using Microsoft.EntityFrameworkCore;
using MiniESS.Core.Projections;
using MiniESS.Infrastructure.Projections;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo.ReadModels;

public class TodoListProjector :
    ProjectorBase<TodoListAggregateRoot>,
    IProject<TodoListEvents.Created>,
    IProject<TodoListEvents.Added>,
    IProject<TodoListEvents.CompletedTodoItem>
{
    public TodoListProjector(TodoDbContext context) : base(context)
    {
    }

    public async Task ProjectEvent(TodoListEvents.Created domainEvent, CancellationToken token)
    {
        var todoList = new TodoList
        {
            Id = domainEvent.StreamId,
            Title = domainEvent.Title,
            TodoItems = new List<TodoItem>()
        };

        await Repository<TodoList>().AddAsync(todoList, token);
        await SaveChangesAsync();
    }

    public async Task ProjectEvent(TodoListEvents.Added domainEvent, CancellationToken token)
    {
        var todoList = await Repository<TodoList>().FindAsync(new object?[] { domainEvent.StreamId }, cancellationToken: token);
        if (todoList is null)
            throw new NotFoundException($"todoList with aggregate id {domainEvent.StreamId} is not found");
            
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

    public async Task ProjectEvent(TodoListEvents.CompletedTodoItem domainEvent, CancellationToken token)
    {
        var todoList = await Repository<TodoList>()
            .Include(x => x.TodoItems)
            .Where(x => x.Id == domainEvent.StreamId)
            .SingleOrDefaultAsync(cancellationToken: token);
        
        if (todoList is null)
            throw new NotFoundException($"todoList with aggregate id {domainEvent.StreamId} is not found");
        
        var todoItem = todoList.TodoItems.SingleOrDefault(x =>
            x.TodoListId == domainEvent.StreamId && x.ItemNumber == domainEvent.ItemNumber);
        
        if (todoItem is null)
            throw new NotFoundException($"todoItem with todoList foreign key id {domainEvent.StreamId} is not found");

        todoItem.IsComplete = true;
        await SaveChangesAsync();
    }
}