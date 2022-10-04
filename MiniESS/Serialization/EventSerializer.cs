using System.Collections.Concurrent;
using System.Drawing;
using System.Reflection;
using System.Text;
using EventStore.Client;
using MiniESS.Aggregate;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MiniESS.Serialization;

public class EventSerializer
{
    private readonly JsonSerializerSettings _settings;
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly ConcurrentDictionary<string, Type> _typesCache;

    public EventSerializer(IEnumerable<Assembly> assemblies)
    {
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };
            
        _assemblies = assemblies;
        _typesCache = new ConcurrentDictionary<string, Type>();
    }

    public IDomainEvent<TKey> Deserialize<TKey>(string type, byte[] data)
    {
        var jsonData = Encoding.UTF8.GetString(data);
        return Deserialize<TKey>(type, jsonData);
    }

    private IDomainEvent<TKey> Deserialize<TKey>(string typeName, string json)
    {
        var typeFromAssembly = GetFromAssembly(typeName);
        if (typeFromAssembly is null)
            throw new InvalidOperationException($"Unknown event type: '{typeName}'");
        
        var eventType = _typesCache.GetOrAdd(typeName, _ => typeFromAssembly);
        return JsonConvert.DeserializeObject(json, eventType, _settings) as IDomainEvent<TKey> 
               ?? throw new InvalidOperationException($"Event: '{typeName}' does not Implement type: '{typeof(IDomainEvent<TKey>)}'"); 
    }

    private Type? GetFromAssembly(string type)
    {
        return _assemblies
                   .Select(x => x.GetType(type, false))
                   .FirstOrDefault(x => x is not null) ??
               Type.GetType(type);
    }

    private class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProp = base.CreateProperty(member, memberSerialization);
            if (jsonProp.Writable) return jsonProp;

            if (member is not PropertyInfo propInfo) 
                return jsonProp;
            
            var setter = propInfo.GetSetMethod(true);
            jsonProp.Writable = setter != null;

            return jsonProp;
        }
    }
}
