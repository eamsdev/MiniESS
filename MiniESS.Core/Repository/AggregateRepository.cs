using EventStore.Client;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Serialization;

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
            aggregateRoot.Events.Select(SerializationHelper.Map), cancellationToken: token);
    }

    private string GetStreamName(Guid aggregateKey)
        => $"{_streamBaseName}_{aggregateKey}";
    
    public async Task<TAggregateRoot?> LoadAsync(Guid key, CancellationToken token)
    {
        var streamName = GetStreamName(key);
        var eventRecord = await _client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: token);
        var events = eventRecord.Select(x => _serializer.Map(x)).ToList();

        return !events.Any() 
            ? null : BaseAggregateRoot<TAggregateRoot>.Create(key, events.OrderBy(e => e.AggregateVersion));
    }
}