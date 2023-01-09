using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Commands;

public abstract class BaseCommand<TAggregateRoot> 
    : ICommand<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
    protected BaseCommand(Guid streamId)
    {
        StreamId = streamId;
    }
    
    public Guid StreamId { get; init; }
}

public interface ICommand<TAggregateRoot> : ICommand 
    where TAggregateRoot : class, IAggregateRoot
{ }


public interface ICommand : IEntityCorrelation
{ 
}