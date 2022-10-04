namespace MiniESS.Aggregate;

public interface IEntity<out TKey>
{
    public TKey StreamId { get; }
}