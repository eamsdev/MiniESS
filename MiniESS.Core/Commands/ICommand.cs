using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Commands;

public interface ICommand<TAggregateRoot> : ICommand 
    where TAggregateRoot : class, IAggregateRoot
{ }

public interface ICommand
{ 
    public Guid AggregateId { get; init; }
}