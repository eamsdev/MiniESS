namespace MiniESS.Todo.Todo;

public class TodoListViewModel
{
    public Guid StreamId { get; init; }
    public string Title { get; init; }
    public List<TodoItemViewModel> TodoItems { get; init; }
}

public class TodoItemViewModel
{
    public int Id { get; init; }
    public bool IsCompleted { get; init; }
    public string Description { get; init; }
}