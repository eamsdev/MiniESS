using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniESS.Subscription;

public class BackgroundWorker : BackgroundService
{
    private readonly ILogger<BackgroundService> _logger;
    private readonly Func<CancellationToken, Task> _task;

    public BackgroundWorker(ILogger<BackgroundService> logger, Func<CancellationToken, Task> task)
    {
        _task = task;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        await Task.Run(async () =>
        {
            await Task.Yield();
            await _task(token);
        }, token);
    }
}