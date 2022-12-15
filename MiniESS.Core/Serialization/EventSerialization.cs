using System.Reflection;
using System.Text;
using MiniESS.Core.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MiniESS.Core.Serialization;

public class EventSerializer
{
    private readonly JsonSerializerSettings _settings;
    private readonly IEnumerable<Assembly> _assemblies;

    public EventSerializer(IEnumerable<Assembly> assemblies)
    {
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        _assemblies = assemblies;
    }

    public IDomainEvent Deserialize(string type, byte[] data)
    {
        var jsonData = Encoding.UTF8.GetString(data);
        return Deserialize(type, jsonData);
    }

    public IDomainEvent Deserialize(string typeName, string json)
    {
        var typeFromAssembly = GetFromAssembly(typeName);
        if (typeFromAssembly is null)
            throw new InvalidOperationException($"Unknown event type: '{typeName}'");
        
        return JsonConvert.DeserializeObject(json, typeFromAssembly, _settings) as IDomainEvent
               ?? throw new InvalidOperationException($"Event: '{typeName}' does not Implement type: '{typeof(IDomainEvent)}'"); 
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