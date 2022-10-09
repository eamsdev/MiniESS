using System;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;

namespace MiniESS.Tests.Models;

public class Dummy : BaseAggregateRoot<Dummy, Guid>
{
    public bool Flag { get; set; }
    public int Count { get; set; }

    private Dummy(Guid id) : base(id)
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

    protected override void Apply(IDomainEvent<Guid> @event)
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
}

public static class DummyEvents
{
    public record DummyCreated : BaseDomainEvent<Dummy, Guid>
    {
        private DummyCreated()
        { }

        public DummyCreated(Dummy dummy) : base(dummy)
        { }
        
    }
    
    public record SetFlag: BaseDomainEvent<Dummy, Guid>
    {
        public bool Flag { get; init; }

        private SetFlag()
        { }

        public SetFlag(Dummy dummy, bool flag) : base(dummy)
        {
            Flag = flag;
        }
        
    }
    
    public record IncrementCounter : BaseDomainEvent<Dummy, Guid>
    {
        private IncrementCounter()
        { }

        public IncrementCounter(Dummy dummy) : base(dummy)
        { }
    }
}