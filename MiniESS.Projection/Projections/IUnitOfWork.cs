namespace MiniESS.Projection.Projections;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}