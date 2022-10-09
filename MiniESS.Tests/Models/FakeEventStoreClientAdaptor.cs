using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MiniESS.Core.Repository;

namespace MiniESS.Tests.Models;

public class FakeEventStoreClientAdaptor : IEventStoreClient
{
    private readonly Dictionary<string, List<EventData>> _eventsMap;
    
    public FakeEventStoreClientAdaptor()
    {
        _eventsMap = new Dictionary<string, List<EventData>>();
    }
    
    public async Task<IWriteResult> AppendToStreamAsync(string streamName, StreamRevision expectedRevision, IEnumerable<EventData> events, CancellationToken token)
    {
        var newEvents = events.ToList();
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

    public IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(Direction dir, string streamName, StreamPosition position, CancellationToken token)
    {
        return _eventsMap[streamName].Select(x => new ResolvedEvent(ToEventRecord(streamName, x), null, null)).ToAsyncEnumerable();
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