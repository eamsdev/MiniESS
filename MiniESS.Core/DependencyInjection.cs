using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Repository;
using MiniESS.Core.Serialization;

namespace MiniESS.Core;

public static class DependencyInjection
{
   public static IServiceCollection AddEventSourcing(
      this IServiceCollection services,
      Action<ConfigurationOption> configureAction)
   {
      var config = ConfigurationOption.Create(configureAction);
      return services
         .AddSingleton(_ => new EventSerializer(config.SerializableAssemblies))
         .AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(config.ConnectionString)))
         .AddSingleton<IEventStoreClient, EventStoreClientAdaptor>();
   }

   public static IServiceCollection AddEventSourcingRepository<TAggregateRoot>(this IServiceCollection services) 
      where TAggregateRoot : class, IAggregateRoot
   {
      return services.AddSingleton<IAggregateRepository<TAggregateRoot>>(sp =>
      {
         var client = sp.GetRequiredService<IEventStoreClient>();
         var serializer = sp.GetRequiredService<EventSerializer>();

         return new AggregateRepository<TAggregateRoot>(client, serializer);
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

   public void WithSerializableAssembly(Assembly assembly)
   {
      SerializableAssemblies.Add(assembly);
   }
}