using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            .AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(config.ConnectionString)))
            .AddSingleton<EventStoreSubscriber>()
            .AddTransient(sp => new EventStoreSubscribe.ToAll(
                sp.GetRequiredService<ILogger<EventStoreSubscribe.ToAll>>(),
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
      
      return config;
   }

   private ConfigurationOption()
   {
      ConnectionString = "";
      HandleAction = null;
   }

   public string ConnectionString { get; set; }

   public Action<ResolvedEvent, CancellationToken>? HandleAction { get; set; }
} 