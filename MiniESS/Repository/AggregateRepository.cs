using MiniESS.Aggregate;
using MiniESS.Serialization;

namespace MiniESS.Repository;

public class AggregateRepository<TAggregateRoot, TKey> : IAggregateRepository<TAggregateRoot, TKey> 
    where TAggregateRoot : class, IAggregateRoot<TKey>
{
    private readonly string _streamBaseName;
    private readonly EventSerializer _serializer;
    private readonly IEventStoreClientAdaptor _client;

    public AggregateRepository(IEventStoreClientAdaptor client, EventSerializer serializer)
    {
        _client = client;
        _serializer = serializer;
        _streamBaseName = typeof(TAggregateRoot).Name;
    }
    
    public Task PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregateRoot> GetAsync(TKey key, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}