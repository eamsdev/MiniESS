using System.Text;
using EventStore.Client;
using MiniESS.Core.Events;
using MiniESS.Core.Serialization;
using Newtonsoft.Json;

namespace MiniESS.Infrastructure.Serialization;

public class EventStoreDbSerializer
{
    private readonly EventSerializer _serializer;

    public EventStoreDbSerializer(EventSerializer serializer)
    {
        _serializer = serializer;
    }

    public IDomainEvent Map(ResolvedEvent resolvedEvent)
    {
        var meta = JsonConvert.DeserializeObject<EventMeta>(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
        return _serializer.Deserialize(meta.EventType, resolvedEvent.Event.Data.ToArray());
    }
    
    public IDomainEvent Map(EventData eventData)
    {
        var meta = JsonConvert.DeserializeObject<EventMeta>(Encoding.UTF8.GetString(eventData.Metadata.ToArray()));
        return _serializer.Deserialize(meta.EventType, eventData.Data.ToArray());
    }
}
