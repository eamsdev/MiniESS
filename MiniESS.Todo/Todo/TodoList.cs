using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;

namespace MiniESS.Todo.Todo;

public class TodoList : BaseAggregateRoot<TodoList>
{
    public TodoList(Guid id) : base(id)
    {
    }

    protected override void Apply(IDomainEvent @event)
    {
        throw new NotImplementedException();
    }
}

public static class TodoListEvents
{
    
}