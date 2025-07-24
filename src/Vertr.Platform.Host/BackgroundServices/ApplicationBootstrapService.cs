namespace Vertr.Platform.Host.BackgroundServices;

public class ApplicationBootstrapService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ApplicationBootstrapService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
