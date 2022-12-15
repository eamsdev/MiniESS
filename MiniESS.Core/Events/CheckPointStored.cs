namespace MiniESS.Core.Events;

public class CheckPointStored
{
    public string SubscriptionId { get; set; }
    public ulong? Position { get; set; }
    public DateTime CheckPointTime { get; set; }
}