using System.Text;
using EventStore.Client;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Events;
using MiniESS.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace MiniESS.Infrastructure.Repository;

public class AggregateRepository<TAggregateRoot> : IAggregateRepository<TAggregateRoot> 
    where TAggregateRoot : class, IAggregateRoot
{
    private readonly string _streamBaseName;
    private readonly EventStoreDbSerializer _serializer;
    private readonly IEventStoreClient _client;

    public AggregateRepository(IEventStoreClient client, EventStoreDbSerializer serializer)
    {
        _client = client;
        _serializer = serializer;
        _streamBaseName = typeof(TAggregateRoot).Name;
    }
    
    public async Task<IWriteResult?> PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token)
    {
        if (aggregateRoot is null)
            throw new ArgumentNullException(nameof(aggregateRoot));

        if (!aggregateRoot.Events.Any())
            return null;

        var expectedRevision = StreamRevision.FromInt64(aggregateRoot.Events.First().AggregateVersion - 1);
        return await _client.AppendToStreamAsync(
            GetStreamName(aggregateRoot.StreamId), 
            expectedRevision,
            aggregateRoot.Events.Select(SerializationHelper.Map), 
            cancellationToken: token);
    }

    public async Task PersistAsyncAndAwaitProjection(TAggregateRoot aggregateRoot, CancellationToken token)
    {
        var persistResult = await PersistAsync(aggregateRoot, token);
        if (persistResult is null)
            return;

        var streamCommitPositionToAwait = persistResult.LogPosition.CommitPosition;
        var foundStreamRevision = (ulong) 0;

        while (foundStreamRevision < streamCommitPositionToAwait)
        {
            // TODO: Remove code duplication -> SubscriptionCheckpointRepository
            var result = await _client.ReadStreamAsync(
                Direction.Backwards, 
                "checkpoint_ToAll", 
                StreamPosition.End, 
                1, 
                cancellationToken: token);
        
            if (!result.Any())
                continue;

            var @event = result.First();
            var eventJson = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
            var streamRevision =  JsonConvert.DeserializeObject<CheckPointStored>(eventJson)?.Position;
            if (streamRevision is null)
                continue;

            foundStreamRevision = streamRevision.Value;
            if (foundStreamRevision >= streamCommitPositionToAwait)
                break;
            
            await Task.Delay(20, token);
        }
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