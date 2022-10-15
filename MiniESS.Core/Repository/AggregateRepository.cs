using System.Text;
using EventStore.Client;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MiniESS.Core.Serialization;
using Newtonsoft.Json;

namespace MiniESS.Core.Repository;

public class AggregateRepository<TAggregateRoot> : IAggregateRepository<TAggregateRoot> 
    where TAggregateRoot : class, IAggregateRoot
{
    private readonly string _streamBaseName;
    private readonly EventSerializer _serializer;
    private readonly IEventStoreClient _client;

    public AggregateRepository(IEventStoreClient client, EventSerializer serializer)
    {
        _client = client;
        _serializer = serializer;
        _streamBaseName = typeof(TAggregateRoot).Name;
    }
    
    public async Task PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token)
    {
        if (aggregateRoot is null)
            throw new ArgumentNullException(nameof(aggregateRoot));

        if (!aggregateRoot.Events.Any())
            return;

        var expectedRevision = StreamRevision.FromInt64(aggregateRoot.Events.First().AggregateVersion - 1);
        await _client.AppendToStreamAsync(GetStreamName(aggregateRoot.StreamId), expectedRevision,
            aggregateRoot.Events.Select(Map), cancellationToken: token);
    }

    private static EventData Map(IDomainEvent @event)
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

    private string GetStreamName(Guid aggregateKey)
        => $"{_streamBaseName}_{aggregateKey}";
    
    public async Task<TAggregateRoot?> LoadAsync(Guid key, CancellationToken token)
    {
        var streamName = GetStreamName(key);
        var readStreamResult = _client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: token);
        var eventRecord = await readStreamResult.ToListAsync(token);
        var events = eventRecord.Select(Map).ToList();

        return !events.Any() 
            ? null : BaseAggregateRoot<TAggregateRoot>.Create(key, events.OrderBy(e => e.AggregateVersion));
    }
    
    private IDomainEvent Map(ResolvedEvent resolvedEvent)
    {
        var meta = JsonConvert.DeserializeObject<EventMeta>(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
        return _serializer.Deserialize(meta.EventType, resolvedEvent.Event.Data.ToArray());
    }
}