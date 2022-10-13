using Microsoft.EntityFrameworkCore;

namespace MiniESS.Subscription.Tests.Models;

public class DummyDbContext : DbContext
{
    public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options)
    { }
    
    public DbSet<DummyReadModel> DummyReadModels { get; set; }
}