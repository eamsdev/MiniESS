using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MiniESS.Todo.Exceptions;
using MiniESS.TodoExample.Todo;

namespace MiniESS.Todo.Todo;

public class TodoListAggregateRoot : BaseAggregateRoot<TodoListAggregateRoot>
{
    private TodoListAggregateRoot(Guid id) : base(id)
    {
        AddEvent(new TodoListEvents.TodoListCreated(this));
    }
    
    public static TodoListAggregateRoot Create(Guid streamId)
    {
        return new TodoListAggregateRoot(streamId);
    }

    public List<TodoItemAggregate> TodoItems { get; set; }

    public void AddTodoItem(string description)
    {
        var nextId = TodoItems.Count;
        AddEvent(new TodoListEvents.TodoItemAdded(this, nextId, description));
    }
    
    public void CompleteTodoItem(int id)
    {
        var toBeCompleted = TodoItems.SingleOrDefault(x => x.Id == id);
        if (toBeCompleted is null)
            throw new DomainException("Todo item does not exist in the todo list.");
        
        AddEvent(new TodoListEvents.TodoItemCompleted(this, id));
    }

    protected override void Apply(IDomainEvent @event)
    { 
        switch (@event)
        {
            case TodoListEvents.TodoItemAdded todoItemAdded:
                TodoItems.Add(TodoItemAggregate.Create(todoItemAdded.Id, todoItemAdded.Description));
                break;
            case TodoListEvents.TodoListCreated _:
                TodoItems = new List<TodoItemAggregate>();
                break;
            case TodoListEvents.TodoItemCompleted todoItemCompleted:
                TodoItems.Single(x => x.Id == todoItemCompleted.Id).Complete();
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
        private TodoListCreated() { }

        public TodoListCreated(TodoListAggregateRoot todoListAggregateRoot) : base(todoListAggregateRoot)
        { }
    }

    public record TodoItemAdded : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int Id { get; }
        public string Description { get; }

        private TodoItemAdded() { }

        public TodoItemAdded(TodoListAggregateRoot todoList, int id, string description) : base(todoList)
        {
            Id = id;
            Description = description;
        }
    }
    
    public record TodoItemCompleted : BaseDomainEvent<TodoListAggregateRoot>
    {
        public int Id { get; }

        private TodoItemCompleted() { }

        public TodoItemCompleted(TodoListAggregateRoot todoList, int id) : base(todoList)
        {
            Id = id;
        }
    }
}