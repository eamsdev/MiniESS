using System.Text;
using EventStore.Client;
using MiniESS.Common.Events;
using Newtonsoft.Json;

namespace MiniESS.Common.Serialization;

public static class SerializationHelper
{
    public static EventData Map<T>(T @event) where T : class
    {
        var json = JsonConvert.SerializeObject(@event, @event.GetType(), null);
        var data = Encoding.UTF8.GetBytes(json);

        var eventType = @event.GetType();
        var meta = new EventMeta
        {
            EventType = eventType.AssemblyQualifiedName!
        };

        var metaJson = JsonConvert.SerializeObject(meta);
        var metaData = Encoding.UTF8.GetBytes(metaJson);

        var eventPayload = new EventData(Uuid.NewUuid(), eventType.Name, data, metaData);
        return eventPayload;
    }
}