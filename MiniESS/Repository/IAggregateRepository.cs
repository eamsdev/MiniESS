using MiniESS.Aggregate;

namespace MiniESS.Repository;

public interface IAggregateRepository<TAggregateRoot, in TKey> where TAggregateRoot : class, IAggregateRoot<TKey>
{
   Task PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token);

   Task<TAggregateRoot?> LoadAsync(TKey key, CancellationToken token);
}