using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Events;
using MiniESS.Core.Serialization;
using MiniESS.Subscription.Subscriptions;
using MiniESS.Subscription.Workers;

namespace MiniESS.Subscription;

public static class DependencyInjection
{
    public static IServiceCollection AddSubscriptionAction(
        this IServiceCollection services,
        Action<ConfigurationOption> configureAction)
    {
        var config = ConfigurationOption.Create(configureAction);
        return services
            .AddScoped(_ => new EventSerializer(config.SerializableAssemblies))
            .AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(config.ConnectionString)))
            .AddSingleton<EventStoreSubscriber>()
            .AddTransient(sp => new EventStoreSubscribe.ToAll(
                sp.GetRequiredService<ILogger<EventStoreSubscribe.ToAll>>(),
                sp.GetRequiredService<EventSerializer>(),
                sp.GetRequiredService<EventStoreSubscriber>(),
                config.HandleAction!))
            .AddHostedService(sp =>
            {
                return new BackgroundWorker(
                    sp.GetRequiredService<ILogger<BackgroundService>>(),
                    token => sp.GetRequiredService<EventStoreSubscribe.ToAll>().SubscribeToAll(token));
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

      if (config.HandleAction is null)
         throw new InvalidOperationException("HandleAction must be defined");
      
      if (!config.SerializableAssemblies.Any())
         throw new InvalidOperationException("No Serializable assemblies provided");
      
      return config;
   }

   private ConfigurationOption()
   {
      ConnectionString = "";
      HandleAction = null;
      SerializableAssemblies = new List<Assembly>();
   }

   public List<Assembly> SerializableAssemblies { get; set; }
   
   public string ConnectionString { get; set; }

   public Action<IDomainEvent, CancellationToken>? HandleAction { get; set; }
} 