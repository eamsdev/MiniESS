using EventStore.Client;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Events;
using MiniESS.Core.Projections;
using MiniESS.Infrastructure.Serialization;
using Polly;

namespace MiniESS.Infrastructure.Subscriptions;

public class EventStoreSubscribeToAll
{
      private readonly EventStoreDbSerializer _serializer;
      private readonly IEventStoreSubscriber _subscriber;
      private readonly ILogger<EventStoreSubscribeToAll> _logger;
      private readonly SubscriptionCheckpointRepository _checkpointRepository;
      private readonly ProjectionOrchestrator _projectionOrchestrator;

      public readonly string SubscriptionId = "ToAll";

      public EventStoreSubscribeToAll(
         EventStoreDbSerializer serializer,
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

      public async Task SubscribeToAll(CancellationToken cancellationToken)
      {
         await Task.Yield(); // see: https://github.com/dotnet/runtime/issues/36063
         var checkpoint = await LoadCheckpoint(cancellationToken);

         if (checkpoint is null)
            _logger.LogInformation("Checkpoint not found, subscribing from the start of the stream");
         else
            _logger.LogInformation("Checkpoint found at position: {}, subscribing from after this position", checkpoint.Value);

         await _subscriber.SubscribeToAllAsync(
            checkpoint is null ? FromAll.Start : FromAll.After(new Position(checkpoint.Value, checkpoint.Value)),
            HandleEvent,
            subscriptionDropped: (subscription, reason, exception)
               => _logger.LogError("Subscription {SubscriptionId}, has been dropped due to {Reason}, exception: {Exception}",
                  subscription.SubscriptionId,
                  reason.ToString(), exception?.Message),
            filterOptions: new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents()),
            cancellationToken: cancellationToken);
      }

      private async Task<ulong?> LoadCheckpoint(CancellationToken cancellationToken)
      {
         ulong? checkpoint = null;
         var retryCheckpointLoadPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[]
         {
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15)
         }, (exception, timeSpan) => {
            _logger.LogWarning("Attempted to subscribe to EventStoreDB, failed due to {}, after {} seconds", exception.GetType().ToString(), timeSpan.TotalSeconds);  
         });
         
         await retryCheckpointLoadPolicy.ExecuteAsync(async () => checkpoint = await _checkpointRepository.Load(SubscriptionId, cancellationToken));
         return checkpoint;
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
         
         if (resolvedEvent.Event.Data.Length is 0 || resolvedEvent.Event.Metadata.Length is 0) 
         {
            _logger.LogTrace("Found event without data, type: {EventType}, dropping this event", resolvedEvent.Event.EventType);
            return;
         }

         IDomainEvent domainEvent;
         try
         {
            domainEvent = _serializer.Map(resolvedEvent);
         }
         catch (Exception)
         {
            _logger.LogTrace("Unable to deserialize event type: {EventType}, id: {StreamId} dropping this event", resolvedEvent.Event.EventType, resolvedEvent.Event.EventStreamId);
            return;
         }
         
         await _projectionOrchestrator.SendToProjector(domainEvent, cancellationToken);
         await _checkpointRepository.Store(SubscriptionId, resolvedEvent.Event.Position.CommitPosition, cancellationToken: cancellationToken);
      }
}