using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MiniESS.Core.Repository;

namespace MiniESS.Core.Tests.Models;

public class FakeEventStoreClientAdaptor : IEventStoreClient
{
    private WrongExpectedVersionException? _throwOnceException;
    private readonly Dictionary<string, List<EventData>> _eventsMap;
    
    public StreamState? StreamMetaDataStreamState { get; set; }
    
    public StreamMetadata? StreamMetaData { get; set; }

    public string StreamMetaDataStreamName { get; set; }
    
    public FakeEventStoreClientAdaptor()
    {
        _eventsMap = new Dictionary<string, List<EventData>>();
    }

    public void ConfigureWrongVersionExceptionOnAppend(
        string streamName,
        StreamRevision expectedStreamRevision,
        StreamRevision actualStreamRevision)
    {
        _throwOnceException = new WrongExpectedVersionException(streamName, expectedStreamRevision, actualStreamRevision);
    }

    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamState expectedState, IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        if (_throwOnceException is not null)
        {
            var tempException = _throwOnceException;
            _throwOnceException = null;
            throw tempException;
        }

        var newEvents = eventData.ToList();
        if (_eventsMap.TryGetValue(streamName, out var storedEvents))
        {
            storedEvents.AddRange(newEvents);
        }
        else
        {
            _eventsMap.Add(streamName, new List<EventData>());
            _eventsMap[streamName].AddRange(newEvents); 
        }
        
        return await Task.FromResult(new SuccessResult());
    }

    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        var newEvents = eventData.ToList();
        if (_eventsMap.TryGetValue(streamName, out var storedEvents))
        {
            storedEvents.AddRange(newEvents);
        }
        else
        {
            _eventsMap.Add(streamName, new List<EventData>());
            _eventsMap[streamName].AddRange(newEvents); 
        }
        
        return await Task.FromResult(new SuccessResult());
    }

    public async Task<List<ResolvedEvent>> ReadStreamAsync(Direction direction, string streamName, StreamPosition revision,
        long maxCount = Int64.MaxValue, bool resolveLinkTos = false, TimeSpan? deadline = null,
        UserCredentials? userCredentials = null, CancellationToken cancellationToken = default)
    {
        if (!_eventsMap.ContainsKey(streamName))
            return new List<ResolvedEvent>();
        
        return _eventsMap[streamName].Select(x => new ResolvedEvent(ToEventRecord(streamName, x), null, null)).ToList();
    }

    public async Task<IWriteResult> SetStreamMetadataAsync(string streamName, StreamState expectedState, StreamMetadata metadata,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null, TimeSpan? deadline = null, UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        StreamMetaDataStreamName = streamName;
        StreamMetaDataStreamState = expectedState;
        StreamMetaData = metadata;
        return await Task.FromResult(new SuccessResult());
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