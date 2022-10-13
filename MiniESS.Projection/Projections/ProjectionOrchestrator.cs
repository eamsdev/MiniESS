using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniESS.Core.Events;
using MiniESS.Projection.Extensions;

namespace MiniESS.Projection.Projections;

public class ProjectionOrchestrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProjectionOrchestrator> _logger;

    public ProjectionOrchestrator(
        IServiceProvider serviceProvider,
        ILogger<ProjectionOrchestrator> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task SendToProjector(IDomainEvent @event, CancellationToken token)
    {
        var aggregateType = @event.GetAssociatedAggregateType();
        if (aggregateType is null)
        {
            _logger.LogWarning("Domain event of type {} is dropped, no associated aggregate type found.", @event.GetType().FullName);
            return;
        }

        var projectorsType = typeof(IProjector<>).MakeGenericType(aggregateType);
        var projectors = _serviceProvider
            .GetServices(projectorsType)
            .Where(x => x is not null)
            .Cast<IProjector>()
            .ToList();

        if (!projectors.Any())
        {
            _logger.LogWarning("Domain event of type {} is dropped, no projector found.", @event.GetType().FullName);
            return;
        }

        foreach (var projector in projectors)
        {
            await projector!.ProjectEventAsync(@event, token);
        }
    }
}