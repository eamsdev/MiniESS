using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace MiniESS.Subscription.Subscriptions;

public class EventStoreSubscribe
{
   public class ToAll
   {
      private readonly ILogger<ToAll> _logger;
      private readonly IEventStoreSubscriber _subscriber;
      private readonly Action<ResolvedEvent, CancellationToken> _handleEventAction;

      public ToAll(
         ILogger<ToAll> logger, 
         IEventStoreSubscriber subscriber,
         Action<ResolvedEvent, CancellationToken> handleEventAction)
      {
         _logger = logger;
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
         _handleEventAction.Invoke(resolvedEvent, token);
         return Task.CompletedTask;
      }
   }
}