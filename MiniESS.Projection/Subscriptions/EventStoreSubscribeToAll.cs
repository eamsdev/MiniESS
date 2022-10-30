using System.Text;
using EventStore.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Events;
using MiniESS.Core.Serialization;
using MiniESS.Projection.Projections;
using MiniESS.Projection.Events;
using Newtonsoft.Json;
using Polly;

namespace MiniESS.Projection.Subscriptions;

public class EventStoreSubscribeToAll
{
      private readonly EventSerializer _serializer;
      private readonly IEventStoreSubscriber _subscriber;
      private readonly ILogger<EventStoreSubscribeToAll> _logger;
      private readonly SubscriptionCheckpointRepository _checkpointRepository;
      private readonly ProjectionOrchestrator _projectionOrchestrator;

      public readonly string SubscriptionId = "ToAll";

      public EventStoreSubscribeToAll(
         EventSerializer serializer,
         IEventStoreSubscriber subscriber,
         ILogger<EventStoreSubscribeToAll> logger,
         ProjectionOrchestrator projectionOrchestrator, 
         SubscriptionCheckpointRepository checkpointRepository)
      {
         _logger = logger;
         _serializer = serializer;
         _subscriber = subscriber;
         _checkpointRepository = checkpointRepository;
         _projectionOrchestrator = projectionOrchestrator;
      }

      private async Task<ulong?> LoadCheckpoint(CancellationToken cancellationToken)
      {
         ulong? checkpoint = null;
         var retryCheckpointLoadPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[]
         {
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15)
         }, (exception, timeSpan) => {
            _logger.LogWarning("Attempted to subscribe to EventStoreDB, failed due to {}, after {} seconds", exception.ToString(), timeSpan.TotalSeconds);  
         });
         
         await retryCheckpointLoadPolicy.ExecuteAsync(async () => checkpoint = await _checkpointRepository.Load(SubscriptionId, cancellationToken));
         
         if (checkpoint is null)
            _logger.LogInformation("Checkpoint not found, subscribing from the start of the stream");
         else
            _logger.LogInformation("Checkpoint found at position: {}, subscribing from after this position", checkpoint.Value);

         return checkpoint;
      }

      public async Task SubscribeToAll(CancellationToken cancellationToken)
      {
         await Task.Yield(); // see: https://github.com/dotnet/runtime/issues/36063
         var checkpoint = await LoadCheckpoint(cancellationToken);

         await _subscriber.SubscribeToAllAsync(
            checkpoint is null ? FromAll.Start : FromAll.After(new Position(checkpoint.Value, checkpoint.Value)), 
            HandleEvent,
            subscriptionDropped: (subscription, reason, _) 
               => _logger.LogError("Subscription {}, has been dropped due to {}.", 
                  subscription.SubscriptionId, 
                  reason.ToString()),
            cancellationToken: cancellationToken);
      }

      private async Task HandleEvent(
         StreamSubscription _,
         ResolvedEvent resolvedEvent,
         CancellationToken cancellationToken)
      {
         if (resolvedEvent.Event.EventType == nameof(CheckPointStored))
         {
            _logger.LogTrace("Found checkpoint event, ignoring this event");
            return;
         }

         await _projectionOrchestrator.SendToProjector(Map(resolvedEvent), cancellationToken);
         await _checkpointRepository.Store(SubscriptionId, resolvedEvent.Event.Position.CommitPosition, cancellationToken: cancellationToken);
      }
      
      private IDomainEvent Map(ResolvedEvent resolvedEvent)
      {
         var meta = JsonConvert.DeserializeObject<EventMeta>(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
         return _serializer.Deserialize(meta.EventType, resolvedEvent.Event.Data.ToArray());
      }
}