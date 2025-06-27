using MediatR;
using Vertr.MarketData.Contracts.Requests;
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
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        var marketDataRequest = new InitialLoadMarketDataRequest();
        var t1 = mediator.Send(marketDataRequest, cancellationToken);

        var portfoliosRequest = new InitialLoadPortfoliosRequest();
        var t2 = mediator.Send(portfoliosRequest, cancellationToken);

        await Task.WhenAll(t1, t2);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
