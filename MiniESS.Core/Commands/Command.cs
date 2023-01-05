using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Commands;

public abstract class BaseCommand<TAggregateRoot> 
    : ICommand<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
    protected BaseCommand(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }
    
    public Guid AggregateId { get; init; }
}

public interface ICommand<TAggregateRoot> : ICommand 
    where TAggregateRoot : class, IAggregateRoot
{ }

public interface ICommand
{ 
    public Guid AggregateId { get; init; }
}