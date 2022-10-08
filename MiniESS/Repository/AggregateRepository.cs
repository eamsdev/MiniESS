using System.Text;
using EventStore.Client;
using MiniESS.Aggregate;
using MiniESS.Events;
using MiniESS.Serialization;

namespace MiniESS.Repository;

public class AggregateRepository<TAggregateRoot, TKey> : IAggregateRepository<TAggregateRoot, TKey> 
    where TAggregateRoot : class, IAggregateRoot<TKey>
{
    private readonly string _streamBaseName;
    private readonly EventSerializer _serializer;
    private readonly IEventStoreClientAdaptor _client;

    public AggregateRepository(IEventStoreClientAdaptor client, EventSerializer serializer)
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
            aggregateRoot.Events.Select(Map), token);
    }

    private static EventData Map(IDomainEvent<TKey> @event)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(@event);
        var data = Encoding.UTF8.GetBytes(json);

        var eventType = @event.GetType();
        var meta = new EventMeta
        {
            EventType = eventType.AssemblyQualifiedName!
        };

        var metaJson = System.Text.Json.JsonSerializer.Serialize(meta);
        var metaData = Encoding.UTF8.GetBytes(metaJson);

        var eventPayload = new EventData(Uuid.NewUuid(), eventType.Name, data, metaData);
        return eventPayload;
    }

    private string GetStreamName(TKey aggregateKey)
        => $"{_streamBaseName}_{aggregateKey}";
    
    public async Task<TAggregateRoot?> GetAsync(TKey key, CancellationToken token)
    {
        var streamName = GetStreamName(key);
        var readStreamResult = _client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, token);
        var eventRecord = await readStreamResult.ToListAsync(token);
        var events = eventRecord.Select(Map).ToList();

        return !events.Any() 
            ? null : BaseAggregateRoot<TAggregateRoot, TKey>.Create(key, events.OrderBy(e => e.AggregateVersion));
    }
    
    private IDomainEvent<TKey> Map(ResolvedEvent resolvedEvent)
    {
        var meta = System.Text.Json.JsonSerializer.Deserialize<EventMeta>(resolvedEvent.Event.Metadata.ToArray());
        return _serializer.Deserialize<TKey>(meta.EventType, resolvedEvent.Event.Data.ToArray());
    }
    
    internal struct EventMeta
    {
        public string EventType { get; set; }
    }
}