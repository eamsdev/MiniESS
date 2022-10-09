using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Repository;

public interface IAggregateRepository<TAggregateRoot, in TKey> where TAggregateRoot : class, IAggregateRoot<TKey>
{
   Task PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token);

   Task<TAggregateRoot?> LoadAsync(TKey key, CancellationToken token);
}