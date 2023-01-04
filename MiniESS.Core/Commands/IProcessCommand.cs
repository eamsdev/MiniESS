namespace MiniESS.Core.Commands;

public interface IProcessCommand<in T> where T : ICommand
{
    Task Process(T command);
}