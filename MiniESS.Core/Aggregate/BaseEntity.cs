namespace MiniESS.Core.Aggregate;

public class BaseEntity : IEntity
{
    protected BaseEntity(Guid streamId)
    {
        StreamId = streamId;
    }

    public Guid StreamId { get; }
}