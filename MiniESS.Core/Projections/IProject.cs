using MiniESS.Core.Events;

namespace MiniESS.Core.Projections;

public interface IProject<in TDomainEvent> where TDomainEvent : class, IDomainEvent
{
    Task ProjectEvent(TDomainEvent domainEvent, CancellationToken token);
}