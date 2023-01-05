namespace MiniESS.Core.Aggregate;

public interface IAggregateRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
   Task<ulong?> PersistAsync(TAggregateRoot aggregateRoot, CancellationToken token);
   
   Task PersistAsyncAndAwaitProjection(TAggregateRoot aggregateRoot, CancellationToken token);

   Task<TAggregateRoot?> LoadAsync(Guid key, CancellationToken token);
}