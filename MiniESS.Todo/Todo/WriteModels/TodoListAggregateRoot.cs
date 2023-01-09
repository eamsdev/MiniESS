using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Core.Events;
using MiniESS.Todo.Exceptions;

namespace MiniESS.Todo.Todo.WriteModels;

public class TodoListAggregateRoot : 
    BaseAggregateRoot<TodoListAggregateRoot>,
    IHandleCommand<TodoListCommands.Create>,
    IHandleCommand<TodoListCommands.AddTodoItem>,
    IHandleCommand<TodoListCommands.CompleteTodoItem>,
    IHandleEvent<TodoListEvents.Created>,
    IHandleEvent<TodoListEvents.Added>,
    IHandleEvent<TodoListEvents.CompletedTodoItem>
{
    private TodoListAggregateRoot(Guid streamId) : base(streamId)
    { }

    public string Title { get; set; }

    public List<TodoItemAggregate> TodoItems { get; set; }

    public void Handle(TodoListCommands.Create command)
    {
        if (command.Title.Length == 0)
            throw new DomainException("Title cannot be null or empty for a Todo List");
        
        RaiseEvent(new TodoListEvents.Created(this, command.Title));
    }

    public void Handle(TodoListCommands.AddTodoItem command)
    {
        RaiseEvent(new TodoListEvents.Added(this, TodoItems.Count, command.Description));
    }

    public void Handle(TodoListCommands.CompleteTodoItem command)
    {
        var toBeCompleted = TodoItems.SingleOrDefault(x => x.ItemNumber == command.ItemNumber) 
                            ?? throw new DomainException("Todo item does not exist in the todo list.");
        
        RaiseEvent(new TodoListEvents.CompletedTodoItem(this, toBeCompleted.ItemNumber));
    }

    public void Handle(TodoListEvents.Created domainEvent)
    {
        Title = domainEvent.Title;
        TodoItems = new List<TodoItemAggregate>();
    }

    public void Handle(TodoListEvents.Added domainEvent)
    {
        TodoItems.Add(TodoItemAggregate.Create(
                    domainEvent.ItemNumber, 
                    domainEvent.Description));
    }

    public void Handle(TodoListEvents.CompletedTodoItem domainEvent)
    {
        TodoItems
            .Single(x => x.ItemNumber == domainEvent.ItemNumber)
            .Complete();
    }
}

public static class TodoListCommands
{
    public class Create : BaseCommand<TodoListAggregateRoot>
    {
        public Create(Guid streamId, string title) : base(streamId)
        {
            Title = title;
        }

        public string Title { get; }
    }
    
    public class AddTodoItem : BaseCommand<TodoListAggregateRoot>
    {
        public AddTodoItem(Guid streamId, string description) : base(streamId)
        {
            Description = description;
        }

        public string Description { get; }
    }
    
    public class CompleteTodoItem : BaseCommand<TodoListAggregateRoot>
    {
        public CompleteTodoItem(Guid streamId, int itemNumber) : base(streamId)
        {
            ItemNumber = itemNumber;
        }

        public int ItemNumber { get; }
    }
}


public static class TodoListEvents
{
    public record Created : BaseDomainEvent<TodoListAggregateRoot>
    {
        public string Title { get; set; } // TODO: Add support for de-serialization without setter 

        private Created() { }

        public Created(TodoListAggregateRoot todoListAggregateRoot, string title) : base(todoListAggregateRoot)
        {
            Title = title;
        }
    }

    public record Added : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int ItemNumber { get; set; }
        public string Description { get; set; }

        private Added() { }

        public Added(TodoListAggregateRoot todoList, int itemNumber, string description) : base(todoList)
        {
            ItemNumber = itemNumber;
            Description = description;
        }
    }
    
    public record CompletedTodoItem : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int ItemNumber { get; set; }

        private CompletedTodoItem() { }

        public CompletedTodoItem(TodoListAggregateRoot todoList, int itemNumber) : base(todoList)
        {
            ItemNumber = itemNumber;
        }
    }
}