using EventStore.Client;

namespace MiniESS.Core.Repository;

public class EventStoreClientAdaptor : IEventStoreClient
{
    protected EventStoreClient Client;

    public EventStoreClientAdaptor(EventStoreClient client)
    {
        Client = client;
    }

    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamState expectedState, IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        return await Client.AppendToStreamAsync(streamName, expectedState, eventData, configureOperationOptions,
            deadline, userCredentials, cancellationToken);
    }

    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        return await Client.AppendToStreamAsync(streamName, expectedRevision, eventData, cancellationToken: cancellationToken);
    }

    public async Task<List<ResolvedEvent>> ReadStreamAsync(Direction direction, string streamName, StreamPosition revision,
        long maxCount = long.MaxValue, bool resolveLinkTos = false, TimeSpan? deadline = null,
        UserCredentials? userCredentials = null, CancellationToken cancellationToken = default)
    {
        var result = Client.ReadStreamAsync(direction, streamName, revision, maxCount,
            cancellationToken: cancellationToken);

        if (await result.ReadState == ReadState.StreamNotFound)
            return new List<ResolvedEvent>();
        
        return await Client.ReadStreamAsync(direction, streamName, revision, maxCount, cancellationToken: cancellationToken).Select(x => x).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IWriteResult> SetStreamMetadataAsync(string streamName, StreamState expectedState, StreamMetadata metadata,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        return await Client.SetStreamMetadataAsync(streamName, expectedState, metadata, cancellationToken: cancellationToken);
    }
}