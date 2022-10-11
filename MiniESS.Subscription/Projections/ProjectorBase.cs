using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MoreLinq;
using SD.Tools.Algorithmia.GeneralDataStructures;

namespace MiniESS.Subscription.Projections;

public abstract class ProjectorBase<TAggregateRoot> : IProjector<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
{
    private readonly DbContext _context;
    private readonly Lazy<MultiValueDictionary<Type, Func<IDomainEvent, Task>>> _projectionHandlers; 
    protected ProjectorBase(DbContext context)
    {
        _context = context;
        _projectionHandlers = new Lazy<MultiValueDictionary<Type, Func<IDomainEvent, Task>>>(CreateHandlers);
    }
    
    public DbSet<T> Repository<T>() where T : class
    {
        return _context.Set<T>();
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task ProjectEventsAsync(Guid aggregateId, IReadOnlyCollection<IDomainEvent> events)
    {
        throw new NotImplementedException();
    }

    private MultiValueDictionary<Type, Func<IDomainEvent, Task>> CreateHandlers()
    {
        var result = new MultiValueDictionary<Type, Func<IDomainEvent, Task>>();
        var actions = GetApplyDelegates();
        actions.ForEach(x => result.Add(x.Type, x.Delegate));
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
                 && x.GetParameters().SingleOrDefault() is not null
                 && typeof(IDomainEvent).IsAssignableFrom(x.GetParameters().First().ParameterType));

        return methodsMatchingSignature.Select(x
            => new TypeDelegatePair(
                x.GetParameters().Single().ParameterType,
                ev => x.Invoke(this, new object[] { ev }) as Task ?? Task.FromException(new NullReferenceException())));
    }

    private readonly record struct TypeDelegatePair(Type Type, Func<IDomainEvent, Task> Delegate);
}