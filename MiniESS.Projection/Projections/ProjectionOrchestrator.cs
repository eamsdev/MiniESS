using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniESS.Common.Events;
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

        using var scope = _serviceProvider.CreateScope();
        var projectorsType = typeof(IProjector<>).MakeGenericType(aggregateType);
        if (scope.ServiceProvider.GetService(projectorsType) is not IProjector projector)
        {
            _logger.LogWarning("Domain event of type {} is dropped, no projector found.", @event.GetType().FullName);
            return;
        }

        await projector.ProjectEventAsync(@event, token);
    }
}