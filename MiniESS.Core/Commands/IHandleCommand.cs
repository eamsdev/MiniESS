namespace MiniESS.Core.Commands;

public interface IHandleCommand<in T> where T : ICommand
{ 
    void Handle(T command);
}