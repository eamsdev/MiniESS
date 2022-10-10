using MiniESS.Core.Aggregate;

namespace MiniESS.Core.Repository;

public interface IAggregateRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
   Task PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token);

   Task<TAggregateRoot?> LoadAsync(Guid key, CancellationToken token);
}