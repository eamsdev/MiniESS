using MiniESS.Todo.Exceptions;

namespace MiniESS.TodoExample.Todo;

public class TodoItemAggregate
{
    public int Id { get; }
    public string Description { get; }
    public bool IsCompleted { get; private set; }
    
    private TodoItemAggregate(int id, string description, bool isCompleted)
    {
        Id = id;
        Description = description;
        IsCompleted = isCompleted;
    }

    public static TodoItemAggregate Create(int id, string description)
    {
        return new TodoItemAggregate(id, description, false);
    }

    public void Complete()
    {
        if (!IsCompleted)
        {
            throw new DomainException("Todo item has already been completed");
        }

        IsCompleted = true;
    }
}