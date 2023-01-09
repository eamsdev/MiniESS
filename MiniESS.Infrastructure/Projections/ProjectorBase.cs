using Microsoft.EntityFrameworkCore;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MiniESS.Core.Projections;

namespace MiniESS.Infrastructure.Projections;

public abstract class ProjectorBase<TAggregateRoot> : IProjector<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
{
    private readonly DbContext _context;
    
    protected ProjectorBase(DbContext context)
    {
        _context = context;
    }

    protected DbSet<T> Repository<T>() where T : class => _context.Set<T>();

    protected async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task ProjectEventAsync(IDomainEvent @event, CancellationToken token)
    {
        var task = typeof(IProject<>)
            .MakeGenericType(@event.GetType())
            .GetMethod(nameof(IProject<IDomainEvent>.ProjectEvent))!
            .Invoke(this, new object[] { @event, token }) as Task;

        await task!;
    }
}