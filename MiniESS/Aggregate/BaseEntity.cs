using System.Globalization;

namespace MiniESS.Aggregate;

public class BaseEntity<TKey> : IEntity<TKey>
{
    protected BaseEntity(TKey id)
    {
        StreamId = id;
    }

    public TKey StreamId { get; }
}