using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Projections;
using MiniESS.Core.Serialization;
using MiniESS.Infrastructure.Repository;
using MiniESS.Infrastructure.Serialization;
using MiniESS.Infrastructure.Subscriptions;
using MiniESS.Infrastructure.Workers;

namespace MiniESS.Infrastructure;

public static class DependencyInjection
{
   public static IServiceCollection AddEventSourcing(
      this IServiceCollection services,
      Action<ConfigurationOption> configureAction)
   {
      var config = ConfigurationOption.Create(configureAction);
      return services
         .AddTransient(_ => new EventSerializer(config.SerializableAssemblies))
         .AddTransient<EventStoreDbSerializer>()
         .AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(config.ConnectionString)))
         .AddTransient<IEventStoreClient, EventStoreClientAdaptor>();
   }

   public static IServiceCollection AddEventSourcingRepository<TAggregateRoot>(this IServiceCollection services) 
      where TAggregateRoot : class, IAggregateRoot
   {
      return services.AddScoped<IAggregateRepository<TAggregateRoot>>(sp =>
      {
         var client = sp.GetRequiredService<IEventStoreClient>();
         var serializer = sp.GetRequiredService<EventStoreDbSerializer>();

         return new AggregateRepository<TAggregateRoot>(client, serializer);
      });
   }
   
   public static IServiceCollection AddProjector<TAggregateType, TConcreteProjector>(this IServiceCollection services) 
      where TConcreteProjector : class, IProjector<TAggregateType>
      where TAggregateType : class, IAggregateRoot
   {
      return services.AddScoped<IProjector<TAggregateType>, TConcreteProjector>();
   }

   public static IServiceCollection AddProjectionService(this IServiceCollection services)
   {
      return services
         .AddLogging(builder =>
         {
            builder.AddConsole();
            builder.AddDebug();
         })
         .AddTransient<EventStoreSubscriber>()
         .AddTransient<ProjectionOrchestrator>()
         .AddTransient<SubscriptionCheckpointRepository>()
         .AddTransient<IEventStoreSubscriber, EventStoreSubscriber>()
         .AddSingleton(sp => new EventStoreSubscribeToAll(
            sp.GetRequiredService<EventStoreDbSerializer>(),
            sp.GetRequiredService<IEventStoreSubscriber>(),
            sp.GetRequiredService<ILogger<EventStoreSubscribeToAll>>(),
            sp.GetRequiredService<ProjectionOrchestrator>(),
            sp.GetRequiredService<SubscriptionCheckpointRepository>())
         )
         .AddHostedService(sp =>
         {
            return new BackgroundWorker(
               token => sp.GetRequiredService<EventStoreSubscribeToAll>().SubscribeToAll(token));
         });
   }
}

public class ConfigurationOption
{
   public static ConfigurationOption Create(Action<ConfigurationOption> configureAction)
   {
      var config = new ConfigurationOption();
      configureAction.Invoke(config);
      
      if (!config.ConnectionString.Any())
         throw new InvalidOperationException("EventStoreDB Connection string must be configured");

      if (!config.SerializableAssemblies.Any())
         throw new InvalidOperationException("No Serializable assemblies provided");
      
      return config;
   }

   private ConfigurationOption()
   {
      ConnectionString = "";
      SerializableAssemblies = new List<Assembly>();
   }

   public string ConnectionString { get; set; }
   public List<Assembly> SerializableAssemblies { get; set; }
}