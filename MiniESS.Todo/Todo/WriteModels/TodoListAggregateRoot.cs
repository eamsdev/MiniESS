using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MiniESS.Todo.Exceptions;

namespace MiniESS.Todo.Todo.WriteModels;

public class TodoListAggregateRoot : BaseAggregateRoot<TodoListAggregateRoot>
{
    private TodoListAggregateRoot(Guid streamId) : base(streamId)
    {
    }

    private TodoListAggregateRoot(Guid streamId, string title) : base(streamId)
    {
        AddEvent(new TodoListEvents.TodoListCreated(this, title));
    }

    public static TodoListAggregateRoot Create(Guid streamId, string title)
    {
        return new TodoListAggregateRoot(streamId, title);
    }

    public string Title { get; set; }

    public List<TodoItemAggregate> TodoItems { get; set; }

    public void AddTodoItem(string description)
    {
        var nextItemNumber = TodoItems.Count;
        AddEvent(new TodoListEvents.TodoItemAdded(this, nextItemNumber, description));
    }
    
    public void CompleteTodoItem(int itemNumber)
    {
        var toBeCompleted = TodoItems.SingleOrDefault(x => x.ItemNumber == itemNumber);
        if (toBeCompleted is null)
            throw new DomainException("Todo item does not exist in the todo list.");
        
        AddEvent(new TodoListEvents.TodoItemCompleted(this, itemNumber));
    }

    protected override void Apply(IDomainEvent @event)
    { 
        switch (@event)
        {
            case TodoListEvents.TodoItemAdded todoItemAdded:
                TodoItems.Add(TodoItemAggregate.Create(todoItemAdded.ItemNumber, todoItemAdded.Description));
                break;
            case TodoListEvents.TodoListCreated todoItemCreated:
                Title = todoItemCreated.Title;
                TodoItems = new List<TodoItemAggregate>();
                break;
            case TodoListEvents.TodoItemCompleted todoItemCompleted:
                TodoItems.Single(x => x.ItemNumber == todoItemCompleted.ItemNumber).Complete();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(@event));
        }
    }
}

public static class TodoListEvents
{
    public record TodoListCreated : BaseDomainEvent<TodoListAggregateRoot>
    {
        public string Title { get; }

        private TodoListCreated() { }

        public TodoListCreated(TodoListAggregateRoot todoListAggregateRoot, string title) : base(todoListAggregateRoot)
        {
            Title = title;
        }
    }

    public record TodoItemAdded : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int ItemNumber { get; }
        public string Description { get; }

        private TodoItemAdded() { }

        public TodoItemAdded(TodoListAggregateRoot todoList, int itemNumber, string description) : base(todoList)
        {
            ItemNumber = itemNumber;
            Description = description;
        }
    }
    
    public record TodoItemCompleted : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int ItemNumber { get; }

        private TodoItemCompleted() { }

        public TodoItemCompleted(TodoListAggregateRoot todoList, int itemNumber) : base(todoList)
        {
            ItemNumber = itemNumber;
        }
    }
}