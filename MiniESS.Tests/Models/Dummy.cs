using System;
using System.Threading.Tasks;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Core.Events;

namespace MiniESS.Subscription.Tests.Models;

public class CreateDummy : ICommand<Dummy>
{
    public Guid AggregateId { get; init; }
}

public class Dummy : 
    BaseAggregateRoot<Dummy>,
    IHandleCommand<CreateDummy>
{
    public bool Flag { get; set; }
    public int Count { get; set; }

    private Dummy(Guid streamId) : base(streamId)
    {
        AddEvent(new DummyEvents.DummyCreated(this));
    }

    public void SetFlag(bool flag)
    {
        AddEvent(new DummyEvents.SetFlag(this, flag)); 
    }

    public void IncrementCount()
    {
        AddEvent(new DummyEvents.IncrementCounter(this)); 
    }

    protected override void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case DummyEvents.DummyCreated dc:
                break;
            case DummyEvents.IncrementCounter ic:
                Count++;
                break;
            case DummyEvents.SetFlag sf:
                Flag = sf.Flag;
                break;
        }
    }

    public static Dummy Create(Guid streamId)
    {
        return new Dummy(streamId);
    }

    public void Handle(CreateDummy command)
    {
        AddEvent(new DummyEvents.DummyCreated(this));
    }
}

public static class DummyEvents
{
    public record DummyCreated : BaseDomainEvent<Dummy>
    {
        private DummyCreated()
        { }

        public DummyCreated(Dummy dummy) : base(dummy)
        { }
        
    }
    
    public record SetFlag: BaseDomainEvent<Dummy>
    {
        public bool Flag { get; init; }

        private SetFlag()
        { }

        public SetFlag(Dummy dummy, bool flag) : base(dummy)
        {
            Flag = flag;
        }
        
    }
    
    public record IncrementCounter : BaseDomainEvent<Dummy>
    {
        private IncrementCounter()
        { }

        public IncrementCounter(Dummy dummy) : base(dummy)
        { }
    }
}