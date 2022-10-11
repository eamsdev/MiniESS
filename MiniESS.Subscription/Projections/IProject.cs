using MiniESS.Core.Events;

namespace MiniESS.Subscription.Projections;

public interface IProject<in TDomainEvent> where TDomainEvent : class, IDomainEvent
{
    void ProjectEvent(TDomainEvent domainEvent);
}