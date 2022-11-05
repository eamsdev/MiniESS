using EventStore.Client;
using MiniESS.Core.Repository;
using MiniESS.Core.Serialization;
using MiniESS.Projection.Projections;
using SD.Tools.Algorithmia.GeneralDataStructures;

namespace MiniESS.Todo.Tests.Utils;

public class EventStoreBypassClient : IEventStoreClient
{
    private readonly EventSerializer _serializer;
    private readonly ProjectionOrchestrator _orchestrator;
    private readonly MultiValueDictionary<string, (EventData Data, StreamRevision Revision)> _eventsRecord;

    public EventStoreBypassClient(
        EventSerializer serializer, 
        ProjectionOrchestrator orchestrator)
    {
        _serializer = serializer;
        _orchestrator = orchestrator;
        _eventsRecord = new MultiValueDictionary<string, (EventData Data, StreamRevision Revision)>();
    }
    
    public Task<IWriteResult> AppendToStreamAsync(string streamName, StreamState expectedState, IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        var expectedRevisionNumber = expectedRevision.ToInt64();
        foreach (var @event in eventData)
        {
            _eventsRecord.Add(streamName, (@event, StreamRevision.FromInt64(++expectedRevisionNumber)));
            await _orchestrator.SendToProjector(_serializer.Map(@event), cancellationToken);
        }
        
        return await Task.FromResult(new SuccessResult());
    }

    public async Task<List<ResolvedEvent>> ReadStreamAsync(Direction direction, string streamName, StreamPosition revision, long maxCount = Int64.MaxValue,
        bool resolveLinkTos = false, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        if (direction is Direction.Backwards)
        {
            throw new NotImplementedException();
        }

        if (!_eventsRecord.TryGetValue(streamName, out var data))
            return await Task.FromResult(new List<ResolvedEvent>());
        
        var sortedEvents = data
            .OrderBy(x => x.Revision.ToInt64())
            .Select(x => new ResolvedEvent(ToEventRecord(streamName, x.Data), null, null))
            .ToList();
            
        return await Task.FromResult(sortedEvents);

    }

    public Task<IWriteResult> SetStreamMetadataAsync(string streamName, StreamState expectedState, StreamMetadata metadata,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    private static EventRecord ToEventRecord(string streamName, EventData eventData)
    {
        return new EventRecord(
            streamName,
            eventData.EventId,
            StreamPosition.Start, // Dont Care
            Position.Start, // Dont Care
            new Dictionary<string, string> { {"type", "foobar"}, {"content-type", "foobar"}, {"created", "0"}}, 
            eventData.Data,
            eventData.Metadata);
    }
}