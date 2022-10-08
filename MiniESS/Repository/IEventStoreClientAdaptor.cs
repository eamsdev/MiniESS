using EventStore.Client;

namespace MiniESS.Repository;

public interface IEventStoreClientAdaptor
{
    Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> events, CancellationToken token);

    IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(Direction dir, string streamName, StreamPosition position,
        CancellationToken token);
}