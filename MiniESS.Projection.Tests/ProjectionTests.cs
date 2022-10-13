using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MiniESS.Core.Events;
using MiniESS.Core.Tests.Models;
using MiniESS.Projection;
using MiniESS.Subscription.Tests.Extensions;
using MiniESS.Subscription.Tests.Models;

namespace MiniESS.Subscription.Tests;

public class ProjectionTests
{
    private readonly DummyDbContext _dbContext;
    private readonly ServiceProvider _serviceProvider;

    public ProjectionTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddProjectionService(option =>
            {
                option.ConnectionString = "dont care lol";
                option.SerializableAssemblies = new List<Assembly> { typeof(Dummy).Assembly };
            })
            .UseStubbedEventStoreSubscriberAndInMemeoryDbContext()
            .BuildServiceProvider();

        _dbContext = _serviceProvider.GetRequiredService<DummyDbContext>();
    }
}