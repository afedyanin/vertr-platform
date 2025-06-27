using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.Contracts.Requests;

namespace Vertr.MarketData.Application.RequestHandlers;

internal class InitialLoadMarketDataHandler : IRequestHandler<InitialLoadMarketDataRequest>
{
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;

    public InitialLoadMarketDataHandler(
        IStaticMarketDataProvider staticMarketDataProvider)
    {
        _staticMarketDataProvider = staticMarketDataProvider;
    }

    public async Task Handle(InitialLoadMarketDataRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implement loading intraday candles
        await _staticMarketDataProvider.InitialLoad();
    }
}
