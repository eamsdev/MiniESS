using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Commands;

public class CommandProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public CommandProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task ProcessAndCommit<T>(ICommand<T> command, CancellationToken cancellationToken) where T : class, IAggregateRoot
    {
        var aggregateRepositoryType = typeof(IAggregateRepository<>)
            .MakeGenericType(typeof(T));

        if (_serviceProvider.GetService(aggregateRepositoryType) is not IAggregateRepository<T> aggregateRepository)
        {
            throw new InvalidOperationException($"AggregateRepository of type {aggregateRepositoryType.FullName} " +
                                                "has not been registered to the IOC");
        }

        var aggregate = await aggregateRepository.LoadAsync(command.StreamId, cancellationToken);
        aggregate ??= BaseAggregateRoot<T>.Create(command.StreamId);

        typeof(IHandleCommand<>)
            .MakeGenericType(command.GetType())
            .GetMethod(nameof(IHandleCommand<ICommand>.Handle))!
            .Invoke(aggregate, new []{ command });
        
        await aggregateRepository.PersistAsyncAndAwaitProjection(aggregate, cancellationToken);
    }
}