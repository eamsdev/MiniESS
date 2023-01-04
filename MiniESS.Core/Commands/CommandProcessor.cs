using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Commands;

public interface ICommandProcessor
{
    Task ProcessAndCommit<T>(ICommand<T> command) where T : class, IAggregateRoot;
}

public class CommandProcessor : ICommandProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public CommandProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task ProcessAndCommit<T>(ICommand<T> command) where T : class, IAggregateRoot
    {
        // TODO:
        // 1. Get Aggregate Type
        // 2. Resolve Aggregate Repository from Service Provider 
        // 3. Retrieve Aggregate from Repository, base on Aggregate Id
        // 4. Via Reflection, get method for processing commands, call said method
        // 5. AggregateRoot will need to inherit IProcessCommand<> which will emit event within
        // 6. Persist
        throw new NotImplementedException();
    }
}