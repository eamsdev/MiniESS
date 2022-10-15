using System.Text;
using EventStore.Client;
using MiniESS.Core.Events;
using MiniESS.Core.Repository;
using MiniESS.Core.Serialization;
using MiniESS.Projection.Events;
using Newtonsoft.Json;

namespace MiniESS.Projection.Subscriptions;

public class SubscriptionCheckpointRepository
{
    private readonly EventSerializer _serializer;
    private readonly IEventStoreClient _eventStoreClient;

    public SubscriptionCheckpointRepository(IEventStoreClient eventStoreClient, EventSerializer serializer)
    {
        _serializer = serializer;
        _eventStoreClient = eventStoreClient;
    }

    private static string GetCheckpointStreamName(string subscriptionId)
    {
        return $"checkpoint_{subscriptionId}";
    }

    public async Task<ulong?> Load(string subscriptionId, CancellationToken token)
    {
        var streamName = GetCheckpointStreamName(subscriptionId);
        var result = await _eventStoreClient.ReadStreamAsync(
            Direction.Backwards, 
            streamName, 
            StreamPosition.End, 
            1, 
            cancellationToken: token).ToListAsync(cancellationToken: token);

        if (!result.Any())
            return null;

        var @event = result.First();
        var eventJson = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
        return JsonConvert.DeserializeObject<CheckPointStored>(eventJson)?.Position;
    }

    public async Task Store(string subscriptionId, ulong position, CancellationToken cancellationToken)
    {
        var @event = new CheckPointStored
        {
            SubscriptionId = subscriptionId,
            Position = position,
            CheckPointTime = DateTime.UtcNow
        };

        var eventToAppend = new[] { Map(@event) };
        var streamName = GetCheckpointStreamName(subscriptionId);

        try
        {
            await _eventStoreClient.AppendToStreamAsync(
                streamName, 
                StreamState.StreamExists, 
                eventToAppend,
                cancellationToken: cancellationToken);
        }
        catch (WrongExpectedVersionException)
        {
            await _eventStoreClient.SetStreamMetadataAsync(
                streamName,
                StreamState.NoStream,
                new StreamMetadata(maxCount: 1),
                cancellationToken: cancellationToken);

            await _eventStoreClient.AppendToStreamAsync(
                streamName,
                StreamState.NoStream,
                eventToAppend,
                cancellationToken: cancellationToken);
        }
    }

    private static EventData Map(CheckPointStored @event)
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