using MiniESS.Todo.Exceptions;

namespace MiniESS.Todo.Todo.WriteModels;

public class TodoItemAggregate
{
    public int ItemNumber { get; }
    public string Description { get; }
    public bool IsCompleted { get; private set; }
    
    private TodoItemAggregate(int itemNumber, string description, bool isCompleted)
    {
        ItemNumber = itemNumber;
        Description = description;
        IsCompleted = isCompleted;
    }

    public static TodoItemAggregate Create(int id, string description)
    {
        return new TodoItemAggregate(id, description, false);
    }

    public void Complete()
    {
        if (IsCompleted)
        {
            throw new DomainException("Todo item has already been completed");
        }

        IsCompleted = true;
    }
}