namespace MiniESS.Core.Events;

public interface IHandleEvent<in T> where T : IDomainEvent
{ 
    void Handle(T domainEvent);
}