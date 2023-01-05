using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;

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
        // TODO:
        // 1. Get Aggregate Type -> typeof(T)
        // 2. Resolve Aggregate Repository from Service Provider 
        var aggregateRepositoryType = typeof(IAggregateRepository<>).MakeGenericType(typeof(T));

        if (_serviceProvider.GetService(aggregateRepositoryType) is not IAggregateRepository<T> aggregateRepository)
        {
            throw new InvalidOperationException($"AggregateRepository of type {aggregateRepositoryType.FullName} " +
                                                "has not been registered to the IOC");
        }

        // 3. Retrieve Aggregate from Repository, base on Aggregate Id
        var aggregate = await aggregateRepository.LoadAsync(command.AggregateId, cancellationToken);
        aggregate ??= BaseAggregateRoot<T>.Create(command.AggregateId);

        // 4. Via Reflection, get method for processing commands, call said method
        var commandHandlerType = typeof(IHandleCommand<>).MakeGenericType(command.GetType());
        var method = commandHandlerType.GetMethod(nameof(IHandleCommand<ICommand>.Handle));
        method.Invoke(aggregate, new []{ command });
        // 5. AggregateRoot will need to inherit IProcessCommand<> which will emit event within
        // 6. Persist
        await aggregateRepository.PersistAsyncAndAwaitProjection(aggregate, cancellationToken);
    }
}