using Microsoft.EntityFrameworkCore;
using MiniESS.Todo.Todo.ReadModels;

namespace MiniESS.Todo.Configuration;

public static class AppConfiguration
{
    public static async Task BootstrapDbContext(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var todoDbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        
        if (todoDbContext.Database.IsRelational())
            await todoDbContext.Database.MigrateAsync();
    }
}