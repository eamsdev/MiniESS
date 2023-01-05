using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Core.Events;
using MiniESS.Todo.Exceptions;

namespace MiniESS.Todo.Todo.WriteModels;

public static class TodoListCommands
{
    public class Create : BaseCommand<TodoListAggregateRoot>
    {
        public Create(Guid aggregateId, string title) : base(aggregateId)
        {
            Title = title;
        }

        public string Title { get; }
    }
    
    public class AddTodoItem : BaseCommand<TodoListAggregateRoot>
    {
        public AddTodoItem(Guid aggregateId, string description) : base(aggregateId)
        {
            Description = description;
        }

        public string Description { get; }
    }
    
    public class CompleteTodoItem : BaseCommand<TodoListAggregateRoot>
    {
        public CompleteTodoItem(Guid aggregateId, int itemNumber) : base(aggregateId)
        {
            ItemNumber = itemNumber;
        }

        public int ItemNumber { get; }
    }
}

public class TodoListAggregateRoot : 
    BaseAggregateRoot<TodoListAggregateRoot>,
    IHandleCommand<TodoListCommands.Create>,
    IHandleCommand<TodoListCommands.AddTodoItem>,
    IHandleCommand<TodoListCommands.CompleteTodoItem>,
    IHandleEvent<TodoListEvents.TodoListCreated>,
    IHandleEvent<TodoListEvents.TodoItemAdded>,
    IHandleEvent<TodoListEvents.TodoItemCompleted>
{
    private TodoListAggregateRoot(Guid streamId) : base(streamId)
    { }

    public string Title { get; set; }

    public List<TodoItemAggregate> TodoItems { get; set; }

    public void Handle(TodoListCommands.Create command)
    {
        if (command.Title.Length == 0)
            throw new DomainException("Title cannot be null or empty for a Todo List");
        
        
        AddEvent(new TodoListEvents.TodoListCreated(this, command.Title));
    }

    public void Handle(TodoListCommands.AddTodoItem command)
    {
        AddEvent(new TodoListEvents.TodoItemAdded(this, TodoItems.Count, command.Description));
    }

    public void Handle(TodoListCommands.CompleteTodoItem command)
    {
        var toBeCompleted = TodoItems.SingleOrDefault(x => x.ItemNumber == command.ItemNumber) 
                            ?? throw new DomainException("Todo item does not exist in the todo list.");
        
        AddEvent(new TodoListEvents.TodoItemCompleted(this, toBeCompleted.ItemNumber));
    }

    public void Handle(TodoListEvents.TodoListCreated domainEvent)
    {
        Title = domainEvent.Title;
        TodoItems = new List<TodoItemAggregate>();
    }

    public void Handle(TodoListEvents.TodoItemAdded domainEvent)
    {
        TodoItems.Add(TodoItemAggregate.Create(
                    domainEvent.ItemNumber, 
                    domainEvent.Description));
    }

    public void Handle(TodoListEvents.TodoItemCompleted domainEvent)
    {
        TodoItems
            .Single(x => x.ItemNumber == domainEvent.ItemNumber)
            .Complete();
    }
}

public static class TodoListEvents
{
    public record TodoListCreated : BaseDomainEvent<TodoListAggregateRoot>
    {
        public string Title { get; set; } // TODO: Add support for de-serialization without setter 

        private TodoListCreated() { }

        public TodoListCreated(TodoListAggregateRoot todoListAggregateRoot, string title) : base(todoListAggregateRoot)
        {
            Title = title;
        }
    }

    public record TodoItemAdded : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int ItemNumber { get; set; }
        public string Description { get; set; }

        private TodoItemAdded() { }

        public TodoItemAdded(TodoListAggregateRoot todoList, int itemNumber, string description) : base(todoList)
        {
            ItemNumber = itemNumber;
            Description = description;
        }
    }
    
    public record TodoItemCompleted : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int ItemNumber { get; set; }

        private TodoItemCompleted() { }

        public TodoItemCompleted(TodoListAggregateRoot todoList, int itemNumber) : base(todoList)
        {
            ItemNumber = itemNumber;
        }
    }
}