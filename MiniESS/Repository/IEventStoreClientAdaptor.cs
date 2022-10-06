using EventStore.Client;

namespace MiniESS.Repository;

public interface IEventStoreClientAdaptor
{
    Task<IWriteResult> AppendToStreamAsync(string streamId, StreamRevision expectedRevision, IEnumerable<EventData> events, CancellationToken token);

    EventStoreClient.ReadStreamResult ReadStreamAsync(Direction dir, string streamName, StreamPosition position, CancellationToken token);
}