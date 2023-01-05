using System;
using System.Threading.Tasks;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Core.Events;

namespace MiniESS.Subscription.Tests.Models;

public class CreateDummy : BaseCommand<Dummy>
{
    public CreateDummy(Guid aggregateId) : base(aggregateId)
    { }
}

public class SetDummyFlag : BaseCommand<Dummy>
{
    public SetDummyFlag(Guid aggregateId, bool flag) : base(aggregateId)
    {
        Flag = flag;
    }
    
    public bool Flag { get; }
}

public class IncrementDummyCount : BaseCommand<Dummy>
{   public IncrementDummyCount(Guid aggregateId) : base(aggregateId)
    { }
    
}

public class Dummy : 
    BaseAggregateRoot<Dummy>,
    IHandleCommand<CreateDummy>,
    IHandleCommand<SetDummyFlag>,
    IHandleCommand<IncrementDummyCount>
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

    public void Handle(SetDummyFlag command)
    {
        Flag = command.Flag;
        AddEvent(new DummyEvents.SetFlag(this, Flag)); 
    }

    public void Handle(IncrementDummyCount command)
    {
        Count++;
        AddEvent(new DummyEvents.IncrementCounter(this)); 
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