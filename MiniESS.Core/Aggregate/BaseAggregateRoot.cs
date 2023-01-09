using System.Reflection;
using MiniESS.Core.Events;

namespace MiniESS.Core.Aggregate;

public abstract class BaseAggregateRoot<TAggregateRoot> : BaseEntity, IAggregateRoot where TAggregateRoot : class, IAggregateRoot
{
    private readonly Queue<IDomainEvent> _eventsQueue;

    protected BaseAggregateRoot(Guid streamId) : base(streamId)
    {
        _eventsQueue = new Queue<IDomainEvent>();
    }

    public long Version { get; set; }
    
    public IReadOnlyCollection<IDomainEvent> Events => _eventsQueue.ToList();
    
    public void ClearEvents() => _eventsQueue.Clear();

    protected void AddEvent(IDomainEvent @event)
    {
        _eventsQueue.Enqueue(@event);
        
        ApplyEvent(this, @event);
        
        Version++;
    }

    /*
     *  Static constructor via reflection to allow subclasses to be created generically
     */
    private static readonly ConstructorInfo? CTor;
    
    static BaseAggregateRoot()
    {
        CTor = typeof(TAggregateRoot).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic, 
            null, 
            new []{ typeof(Guid) }, 
            null);
        
        if (null == CTor)
            throw new InvalidOperationException($"Unable to find required private constructor with param of type '{typeof(Guid)}', for Aggregate of type '{typeof(TAggregateRoot)}'");
    }

    public static TAggregateRoot Create(Guid key)
    {
        return (TAggregateRoot) CTor.Invoke(new object [] { key });
    }

    public static TAggregateRoot Create(Guid key, IEnumerable<IDomainEvent> events)
    {
        var domainEvents = events as IDomainEvent[] ?? events.ToArray();
        if (null == events || !domainEvents.Any() || CTor is null)
            throw new ArgumentNullException();
        
        var result = (TAggregateRoot) CTor.Invoke(new object [] { key });
        if (result is BaseAggregateRoot<TAggregateRoot> baseAggregate)
        {
            foreach (var @event in domainEvents)
            {
                ApplyEvent(baseAggregate, @event);
                baseAggregate.Version++;
            }
        }
        
        result.ClearEvents();
        return result;
    }

    private static void ApplyEvent(
        IEntity aggregate, 
        IDomainEvent @event) 
    => typeof(IHandleEvent<>)
        .MakeGenericType(@event.GetType())
        .GetMethod(nameof(IHandleEvent<IDomainEvent>.Handle))!
        .Invoke(aggregate, new []{ @event });
    
}