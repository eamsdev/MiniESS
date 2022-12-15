using Microsoft.Extensions.Hosting;

namespace MiniESS.Infrastructure.Workers;

public class BackgroundWorker : BackgroundService
{
    private readonly Func<CancellationToken, Task> _task;

    public BackgroundWorker(Func<CancellationToken, Task> task)
    {
        _task = task;
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