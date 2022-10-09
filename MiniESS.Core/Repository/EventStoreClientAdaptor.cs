using EventStore.Client;

namespace MiniESS.Core.Repository;

public class EventStoreClientAdaptor : IEventStoreClient
{
    protected EventStoreClient Client;

    public EventStoreClientAdaptor(EventStoreClient client)
    {
        Client = client;
    }
    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> events, CancellationToken token)
    {
        return await Client.AppendToStreamAsync(streamName, expectedRevision, events, cancellationToken: token);
    }

    public IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(Direction dir, string streamName, StreamPosition position,
        CancellationToken token)
    {
        return Client.ReadStreamAsync(dir, streamName, position, cancellationToken: token).Select(x => x);
    }
}