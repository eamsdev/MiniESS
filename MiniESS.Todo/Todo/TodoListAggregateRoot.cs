using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MiniESS.Todo.Exceptions;
using MiniESS.TodoExample.Todo;

namespace MiniESS.Todo.Todo;

public class TodoListAggregateRoot : BaseAggregateRoot<TodoListAggregateRoot>
{
    public TodoListAggregateRoot(Guid id) : base(id)
    {
        AddEvent(new TodoListEvents.TodoListCreated(this));
    }

    public List<TodoItemAggregate> TodoItems { get; set; }

    protected override void Apply(IDomainEvent @event)
    { 
        switch (@event)
        {
            case TodoListEvents.TodoItemAdded todoItemAdded:
                var nextId = TodoItems.Count;
                var toBeAdded = TodoItemAggregate.Create(nextId, todoItemAdded.Description);
                TodoItems.Add(toBeAdded);
                break;
            case TodoListEvents.TodoListCreated todoListCreated:
                TodoItems = new List<TodoItemAggregate>();
                break;
            case TodoListEvents.TodoItemCompleted todoItemCompleted:
                var toBeCompleted = TodoItems.SingleOrDefault(x => x.Id == todoItemCompleted.Id);
                if (toBeCompleted is null)
                    throw new DomainException("Todo item does not exist in the todo list.");

                toBeCompleted.Complete();
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
        public string Description { get; }

        private TodoItemAdded() { }

        public TodoItemAdded(TodoListAggregateRoot todoList, string description) : base(todoList)
        {
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