using System.ComponentModel.DataAnnotations;

namespace MiniESS.Todo.Todo.ReadModels;

public class TodoList
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    public ICollection<TodoItem> TodoItems { get; set; }
}