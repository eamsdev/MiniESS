using EventStore.Client;

namespace MiniESS.Repository;

public class EventStoreClientAdaptor : IEventStoreClientAdaptor
{
    private readonly EventStoreClient _client;

    public EventStoreClientAdaptor(EventStoreClient client)
    {
        _client = client;
    }
    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> events, CancellationToken token)
    {
        return await _client.AppendToStreamAsync(streamName, expectedRevision, events, cancellationToken: token);
    }

    public IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(Direction dir, string streamName, StreamPosition position,
        CancellationToken token)
    {
        return _client.ReadStreamAsync(dir, streamName, position, cancellationToken: token).Select(x => x);
    }
}