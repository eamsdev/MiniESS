using System.Text;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Events;
using MiniESS.Core.Serialization;
using Newtonsoft.Json;

namespace MiniESS.Subscription.Subscriptions;

public class EventStoreSubscribe
{
   public class ToAll
   {
      private readonly ILogger<ToAll> _logger;
      private readonly EventSerializer _serializer;
      private readonly IEventStoreSubscriber _subscriber;
      private readonly Action<IDomainEvent, CancellationToken> _handleEventAction;

      public ToAll(
         ILogger<ToAll> logger, 
         EventSerializer serializer,
         IEventStoreSubscriber subscriber,
         Action<IDomainEvent, CancellationToken>? handleEventAction)
      {
         _logger = logger;
         _serializer = serializer;
         _subscriber = subscriber;
         _handleEventAction = handleEventAction;
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

      private Task HandleEvent(
         StreamSubscription _,
         ResolvedEvent resolvedEvent,
         CancellationToken token)
      {
         _handleEventAction.Invoke(Map(resolvedEvent), token);
         return Task.CompletedTask;
      }
      
      private IDomainEvent Map(ResolvedEvent resolvedEvent)
      {
         var meta = JsonConvert.DeserializeObject<EventMeta>(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
         return _serializer.Deserialize(meta.EventType, resolvedEvent.Event.Data.ToArray());
      }
   }
}