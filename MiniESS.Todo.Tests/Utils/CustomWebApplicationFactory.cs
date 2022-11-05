using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Repository;
using MiniESS.Projection.Workers;
using MiniESS.Todo.Todo.ReadModels;

namespace MiniESS.Todo.Tests.Utils;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Remove(services.Where(d => d.ServiceType == typeof(IHostedService)).Single(d => d.ImplementationFactory?.GetType().ToString() == typeof(Func<IServiceProvider, BackgroundWorker>).ToString()));
            services.Remove(services.Single(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>)));
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDB");
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });
            services.Remove(services.Single(d => d.ServiceType == typeof(IEventStoreClient)));
            services.AddSingleton<IEventStoreClient, EventStoreBypassClient>();

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

            try
            {
                // Tear down and re-create in-memory database, better alternative
                // than spinning up multiple with different names and waste memory
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred creating the in-memory database. Error: {Message}", e.Message);
                throw;
            }
        });
    }
}