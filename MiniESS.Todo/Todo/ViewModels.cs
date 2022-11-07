namespace MiniESS.Todo.Todo;

public class TodoList
{
    public Guid StreamId { get; init; }
    public string Title { get; init; }
    public List<TodoItem> TodoItems { get; init; }
}

public class TodoItem
{
    public int Id { get; init; }
    public bool IsCompleted { get; init; }
    public string Description { get; init; }
}