using System.Text;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Events;
using MiniESS.Core.Serialization;
using MiniESS.Projection.Projections;
using Newtonsoft.Json;

namespace MiniESS.Projection.Subscriptions;

public class EventStoreSubscribeToAll
{
      private readonly EventSerializer _serializer;
      private readonly IEventStoreSubscriber _subscriber;
      private readonly ILogger<EventStoreSubscribeToAll> _logger;
      private readonly ProjectionOrchestrator _projectionOrchestrator;

      public EventStoreSubscribeToAll(
         EventSerializer serializer,
         IEventStoreSubscriber subscriber,
         ILogger<EventStoreSubscribeToAll> logger,
         ProjectionOrchestrator projectionOrchestrator)
      {
         _logger = logger;
         _serializer = serializer;
         _subscriber = subscriber;
         _projectionOrchestrator = projectionOrchestrator;
      }

      // TODO: Add Checkpoint support
      public async Task SubscribeToAll(CancellationToken token)
      {
         await Task.Yield(); // see: https://github.com/dotnet/runtime/issues/36063
         await _subscriber.SubscribeToAllAsync(
            FromAll.Start, 
            HandleEvent, 
            cancellationToken: token);
      }

      private async Task HandleEvent(
         StreamSubscription _,
         ResolvedEvent resolvedEvent,
         CancellationToken token)
      {
         await _projectionOrchestrator.SendToProjector(Map(resolvedEvent), token);
      }
      
      private IDomainEvent Map(ResolvedEvent resolvedEvent)
      {
         var meta = JsonConvert.DeserializeObject<EventMeta>(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
         return _serializer.Deserialize(meta.EventType, resolvedEvent.Event.Data.ToArray());
      }
}