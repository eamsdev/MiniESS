namespace MiniESS;

public interface IEntity<out TKey>
{
    public TKey StreamId { get; }
}