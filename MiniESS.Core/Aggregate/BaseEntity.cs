namespace MiniESS.Core.Aggregate;

public class BaseEntity : IEntity
{
    protected BaseEntity(Guid id)
    {
        StreamId = id;
    }

    public Guid StreamId { get; }
}