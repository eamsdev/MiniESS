namespace MiniESS.Core.Projections;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}