using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Serialization;
using MiniESS.Projection.Projections;
using MiniESS.Projection.Subscriptions;
using MiniESS.Projection.Workers;

namespace MiniESS.Projection;

public static class DependencyInjection
{
    public static IServiceCollection AddProjector<TAggregateType, TConcreteProjector>(this IServiceCollection services) 
        where TConcreteProjector : class, IProjector<TAggregateType>
        where TAggregateType : class, IAggregateRoot
    {
        return services.AddScoped<IProjector<TAggregateType>, TConcreteProjector>();
    }

    public static IServiceCollection AddProjectionService(
        this IServiceCollection services,
        Action<ConfigurationOption> configureAction)
    {
        var config = ConfigurationOption.Create(configureAction);
        return services
            .AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            })
            .AddTransient(_ => new EventSerializer(config.SerializableAssemblies))
            .AddTransient(_ => new EventStoreClient(EventStoreClientSettings.Create(config.ConnectionString)))
            .AddTransient<EventStoreSubscriber>()
            .AddTransient<ProjectionOrchestrator>()
            .AddTransient<SubscriptionCheckpointRepository>()
            .AddTransient<IEventStoreSubscriber, EventStoreSubscriber>()
            .AddSingleton(sp => new EventStoreSubscribeToAll(
                sp.GetRequiredService<EventSerializer>(),
                sp.GetRequiredService<IEventStoreSubscriber>(),
                sp.GetRequiredService<ILogger<EventStoreSubscribeToAll>>(),
                sp.GetRequiredService<ProjectionOrchestrator>(),
                sp.GetRequiredService<SubscriptionCheckpointRepository>())
            )
            .AddHostedService(sp =>
            {
                return new BackgroundWorker(
                    sp.GetRequiredService<ILogger<BackgroundWorker>>(),
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

   public List<Assembly> SerializableAssemblies { get; set; }
   
   public string ConnectionString { get; set; }
} 