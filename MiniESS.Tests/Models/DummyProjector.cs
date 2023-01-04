using System.Threading;
using System.Threading.Tasks;
using MiniESS.Core.Projections;
using MiniESS.Infrastructure.Projections;

namespace MiniESS.Subscription.Tests.Models;

public class DummyProjector : 
    ProjectorBase<Dummy>,
    IProject<DummyEvents.DummyCreated>,
    IProject<DummyEvents.IncrementCounter>
{
    public DummyProjector(DummyDbContext context) : base(context)
    {
    }

    public async Task ProjectEvent(DummyEvents.IncrementCounter domainEvent, CancellationToken token)
    {
        var dummy = await Repository<DummyReadModel>().FindAsync(new object?[] { domainEvent.AggregateId }, cancellationToken: token);
        dummy!.Count++;
        await SaveChangesAsync();
    }

    public async Task ProjectEvent(DummyEvents.DummyCreated domainEvent, CancellationToken token)
    {
        var dummy = new DummyReadModel
        {
            StreamId = domainEvent.AggregateId
        };

        await Repository<DummyReadModel>().AddAsync(dummy, token);
        await SaveChangesAsync();
    }
}