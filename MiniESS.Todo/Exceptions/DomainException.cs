namespace MiniESS.Todo.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    { }
}