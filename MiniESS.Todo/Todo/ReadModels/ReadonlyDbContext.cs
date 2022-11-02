using Microsoft.EntityFrameworkCore;

namespace MiniESS.Todo.Todo.ReadModels;

public class ReadonlyDbContext
{
    private readonly TodoDbContext _dbContext;

    public ReadonlyDbContext(TodoDbContext context)
    {
        _dbContext = context;
    }

    public IQueryable<TEntity> Set<TEntity>() where TEntity : class
    {
        return _dbContext.Set<TEntity>().AsNoTracking();
    }
}