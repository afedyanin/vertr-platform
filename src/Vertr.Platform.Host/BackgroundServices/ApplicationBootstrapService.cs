using MediatR;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.Platform.Host.BackgroundServices;

public class ApplicationBootstrapService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ApplicationBootstrapService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var portfoliosRequest = new InitialLoadPortfoliosRequest();
        await mediator.Send(portfoliosRequest, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
