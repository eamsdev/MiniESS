using EventStore.Client;

namespace MiniESS.Core.Repository;

public interface IEventStoreClient
{
    Task<IWriteResult> AppendToStreamAsync(
        string streamName,
        StreamState expectedState,
        IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<IWriteResult> AppendToStreamAsync(
        string streamName,
        StreamRevision expectedRevision,
        IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<List<ResolvedEvent>> ReadStreamAsync(
        Direction direction,
        string streamName,
        StreamPosition revision,
        long maxCount = long.MaxValue,
        bool resolveLinkTos = false,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<IWriteResult> SetStreamMetadataAsync(
        string streamName,
        StreamState expectedState,
        StreamMetadata metadata,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);
}