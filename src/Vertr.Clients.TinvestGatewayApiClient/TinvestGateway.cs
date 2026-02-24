using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Clients.TinvestGatewayApiClient;

internal sealed class TinvestGateway : ITradingGateway
{
    private readonly ITinvestGatewayClient _gatewayClient;

    public TinvestGateway(ITinvestGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
    }

    public Task<Instrument[]> GetAllInstruments()
        => _gatewayClient.GetAllInstruments();

    public Task<Candle[]> GetCandles(Guid instrumentId, int maxItems = -1)
        => _gatewayClient.GetCandles(instrumentId, maxItems);

    public Task<string?> PostMarketOrder(MarketOrderRequest request)
        => _gatewayClient.PostMarketOrder(request);

    public Task<OrderTrades[]> FindOrderTrades(string orderId)
        => _gatewayClient.FindOrderTrades($"{orderId}.*");

    public Task<Portfolio?> GetPortfolio(Guid portfolioId)
        => _gatewayClient.GetPortfolio(portfolioId);
}
