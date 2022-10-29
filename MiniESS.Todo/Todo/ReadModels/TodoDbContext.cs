using Microsoft.EntityFrameworkCore;

namespace MiniESS.Todo.Todo.ReadModels;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    { }
    
    public DbSet<TodoList> TodoLists { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }
}