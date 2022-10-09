using System.Reflection;
using MiniESS.Core.Events;

namespace MiniESS.Core.Aggregate;

public abstract class BaseAggregateRoot<TAggregateRoot, TKey> : BaseEntity<TKey>, IAggregateRoot<TKey> where TAggregateRoot : class, IAggregateRoot<TKey>
{
    private readonly Queue<IDomainEvent<TKey>> _eventsQueue;

    protected BaseAggregateRoot(TKey id) : base(id)
    {
        _eventsQueue = new Queue<IDomainEvent<TKey>>();
    }

    public long Version { get; set; }
    
    public IReadOnlyCollection<IDomainEvent<TKey>> Events => _eventsQueue.ToList();
    
    public void ClearEvents() => _eventsQueue.Clear();

    protected void AddEvent(IDomainEvent<TKey> @event)
    {
        _eventsQueue.Enqueue(@event);
    
        Apply(@event);
        
        Version++;
    }

    protected abstract void Apply(IDomainEvent<TKey> @event);
   
    /*
     *  Static constructor via reflection to allow subclasses to be created generically
     */
    private static readonly ConstructorInfo? CTor;
    
    static BaseAggregateRoot()
    {
        CTor = typeof(TAggregateRoot).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic, 
            null, 
            new []{ typeof(TKey) }, 
            null);
        
        if (null == CTor)
            throw new InvalidOperationException($"Unable to find required private constructor with param of type '{typeof(TKey)}', for Aggregate of type '{typeof(TAggregateRoot)}'");
    }
        
    public static TAggregateRoot Create(TKey key, IEnumerable<IDomainEvent<TKey>> events)
    {
        var domainEvents = events as IDomainEvent<TKey>[] ?? events.ToArray();
        if (null == events || !domainEvents.Any() || CTor is null || key is null)
            throw new ArgumentNullException();
        
        var result = (TAggregateRoot) CTor.Invoke(new object [] { key });
        if (result is BaseAggregateRoot<TAggregateRoot, TKey> baseAggregate)
        {
            foreach (var @event in domainEvents)
            {
                baseAggregate.AddEvent(@event);
            }
        }
        
        result.ClearEvents();
        return result;
    }
}