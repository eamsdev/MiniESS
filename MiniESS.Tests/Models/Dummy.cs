using System;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Core.Events;

namespace MiniESS.Subscription.Tests.Models;

public class CreateDummy : BaseCommand<Dummy>
{
    public CreateDummy(Guid streamId) : base(streamId)
    { }
}

public class SetDummyFlag : BaseCommand<Dummy>
{
    public SetDummyFlag(Guid streamId, bool flag) : base(streamId)
    {
        Flag = flag;
    }
    
    public bool Flag { get; }
}

public class IncrementDummyCount : BaseCommand<Dummy>
{   public IncrementDummyCount(Guid streamId) : base(streamId)
    { }
    
}

public class Dummy : 
    BaseAggregateRoot<Dummy>,
    IHandleCommand<CreateDummy>,
    IHandleCommand<SetDummyFlag>,
    IHandleCommand<IncrementDummyCount>,
    IHandleEvent<DummyEvents.DummyCreated>,
    IHandleEvent<DummyEvents.IncrementCounter>,
    IHandleEvent<DummyEvents.SetFlag>
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

    public static Dummy Create(Guid streamId)
    {
        return new Dummy(streamId);
    }

    public void Handle(CreateDummy command)
    {
        AddEvent(new DummyEvents.DummyCreated(this));
    }

    public void Handle(SetDummyFlag command)
    {
        AddEvent(new DummyEvents.SetFlag(this, command.Flag)); 
    }

    public void Handle(IncrementDummyCount command)
    {
        AddEvent(new DummyEvents.IncrementCounter(this)); 
    }

    public void Handle(DummyEvents.DummyCreated domainEvent)
    {}

    public void Handle(DummyEvents.IncrementCounter domainEvent)
    {
        Count++;
    }

    public void Handle(DummyEvents.SetFlag domainEvent)
    {
        Flag = domainEvent.Flag;
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