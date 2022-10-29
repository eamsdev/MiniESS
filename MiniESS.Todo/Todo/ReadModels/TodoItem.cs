using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniESS.Todo.Todo.ReadModels;

public class TodoItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }
    
    [Required]
    public int ItemNumber { get; set; }
    
    [Required]
    public bool IsComplete { get; set; }
    
    
    [Required]
    [ForeignKey("TodoList")]
    public Guid TodoListId { get; set; }
    public TodoList TodoList { get; set; }
}