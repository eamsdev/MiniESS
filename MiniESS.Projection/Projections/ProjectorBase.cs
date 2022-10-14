using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MoreLinq;
using SD.Tools.Algorithmia.GeneralDataStructures;

namespace MiniESS.Projection.Projections;

public abstract class ProjectorBase<TAggregateRoot> : IProjector<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
{
    private readonly DbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProjectorBase<TAggregateRoot>> _logger;
    private readonly Lazy<MultiValueDictionary<Type, Func<IDomainEvent, CancellationToken, Task>>> _projectionHandlers;

    protected ProjectorBase(
        DbContext context,
        IServiceProvider serviceProvider,
        ILogger<ProjectorBase<TAggregateRoot>> logger)
    {
        _logger = logger;
        _context = context;
        _serviceProvider = serviceProvider;
        _projectionHandlers = new Lazy<MultiValueDictionary<Type, Func<IDomainEvent, CancellationToken, Task>>>(CreateHandlers);
    }
    
    public DbSet<T> Repository<T>() where T : class
    {
        return _context.Set<T>();
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task ProjectEventAsync(IDomainEvent @event, CancellationToken token)
    {
        if (!_projectionHandlers.Value.TryGetValue(@event.GetType(), out var handlers)) 
            return;
        
        foreach (var handler in handlers)
            await handler.Invoke(@event, token);
    }

    private MultiValueDictionary<Type, Func<IDomainEvent, CancellationToken, Task>> CreateHandlers()
    {
        var result = new MultiValueDictionary<Type, Func<IDomainEvent, CancellationToken, Task>>();
        var actions = GetApplyDelegates();
        MoreEnumerable.ForEach(actions, x => result.Add(x.Type, x.Delegate));
        return result;
    }

    private IEnumerable<TypeDelegatePair> GetApplyDelegates()
    {
        var availableMethods = GetType().GetMethods(
            BindingFlags.Instance 
            | BindingFlags.Public 
            | BindingFlags.NonPublic);

        var methodsMatchingSignature = availableMethods.Where(
            x => x.Name == nameof(IProject<IDomainEvent>.ProjectEvent)
                 && typeof(IDomainEvent).IsAssignableFrom(x.GetParameters().First().ParameterType)).ToList();

        return methodsMatchingSignature.Select(x
            => new TypeDelegatePair(
                x.GetParameters().First().ParameterType,
                (ev, token) => x.Invoke(this, new object[] { ev, token }) as Task ?? Task.FromException(new NullReferenceException())));
    }

    private readonly record struct TypeDelegatePair(Type Type, Func<IDomainEvent, CancellationToken, Task> Delegate);
}